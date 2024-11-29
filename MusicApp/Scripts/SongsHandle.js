// Global audio player object to ensure single instance
window.audioPlayer = {
    audio: new Audio(),
    isPlaying: false,
    currentSongId: null,
};

// Utility functions
window.audioPlayer.playSong = function (songData) {
    if (window.audioPlayer.currentSongId === songData.id) {
        // Toggle play/pause if the same song is clicked
        window.audioPlayer.togglePlayPause();
        return;
    }

    // Set new song and play
    window.audioPlayer.audio.src = "/Public/Songs/" + songData.src;
    window.audioPlayer.audio.play();
    window.audioPlayer.isPlaying = true;
    window.audioPlayer.currentSongId = songData.id;

    // Update footer information
    $(document).trigger('updateFooter', songData);
};

window.audioPlayer.togglePlayPause = function () {
    if (window.audioPlayer.audio.paused) {
        window.audioPlayer.audio.play();
        window.audioPlayer.isPlaying = true;
        $(document).trigger('footerPlaying');
    } else {
        window.audioPlayer.audio.pause();
        window.audioPlayer.isPlaying = false;
        $(document).trigger('footerPaused');
    }
};
