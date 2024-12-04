using System.IO;
using System.Web.Mvc;

namespace MusicApp.Controllers
{
    public class AudioController : Controller
    {
        // GET: Audio
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StreamMp3(string fileName)
        {
            var filePath = Server.MapPath("/Public/Songs/" + fileName + ".mp3"); // Adjust the file path as needed

            // Check if the file exists
            if (!System.IO.File.Exists(filePath))
            {
                return HttpNotFound();
            }

            var fileInfo = new FileInfo(filePath);
            long fileSize = fileInfo.Length;

            // Get the Range header (if available)
            var rangeHeader = Request.Headers["Range"];
            if (rangeHeader != null)
            {
                // Parse the Range header to get the byte range (e.g., "bytes=0-1023")
                var range = rangeHeader.Replace("bytes=", "").Split('-');
                long startRange = long.Parse(range[0]);
                long endRange = (range.Length > 1 && !string.IsNullOrEmpty(range[1]))
                                 ? long.Parse(range[1])
                                 : fileSize - 1;

                // Make sure the end range does not exceed the file size
                if (endRange > fileSize - 1)
                {
                    endRange = fileSize - 1;
                }

                // Set the response status code to 206 (Partial Content)
                Response.StatusCode = 206;
                Response.AddHeader("Content-Range", $"bytes {startRange}-{endRange}/{fileSize}");
                Response.AddHeader("Accept-Ranges", "bytes");
                Response.ContentType = "audio/mpeg";

                // Set the content length of the response to the range size
                Response.AddHeader("Content-Length", (endRange - startRange + 1).ToString());

                // Open the file and write the byte range to the response
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                fileStream.Seek(startRange, SeekOrigin.Begin);

                byte[] buffer = new byte[1024 * 8]; // 8KB buffer
                int bytesRead;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0 && startRange <= endRange)
                {
                    if (startRange + bytesRead > endRange)
                    {
                        bytesRead = (int)(endRange - startRange + 1); // Trim excess bytes
                    }

                    Response.OutputStream.Write(buffer, 0, bytesRead);
                    startRange += bytesRead;
                }
                fileStream.Close();
                Response.End();
                return new EmptyResult(); // Ensure the action returns after streaming
            }
            else
            {
                // If no range is requested, serve the full file (200 OK)
                Response.ContentType = "audio/mpeg";
                Response.AddHeader("Content-Length", fileSize.ToString());
                return File(filePath, "audio/mpeg");
            }
        }
    }
}