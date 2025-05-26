using Exiled.API.Interfaces;


namespace PixelSpray
{
    public class Translations : ITranslation
    {
        public string SprayCommandDescription { get; set; } = "Creates a Spray in-game from the image at the specified URL.";
        public string OnlyPlayerOrConsoleCanUse { get; set; } = "This command can only be used by players or the server console.";
        public string NoPermission { get; set; } = "You do not have the required permission (pixel_spray_spray) to use this command.";
        public string Usage { get; set; } = "Usage: .graffiti <imageUrl>";
        public string RequestReceived { get; set; } = "Your graffiti request has been received ({0}), the image is being processed... Please wait.";
        public string GraffitiEmptyResult { get; set; } = "Graffiti could not be created (empty result).";
        public string GraffitiCommandSent { get; set; } = "Graffiti command sent to the server. Check if it has been created.";
        public string GraffitiCommandError { get; set; } = "An error occurred while creating graffiti (command execution stage).";
        public string ImageDownloadError { get; set; } = "ERROR: Image could not be downloaded. (Network Error: {0})";
        public string GraffitiGeneralError { get; set; } = "ERROR: There was a problem creating the graffiti.";
        public string GraffitiRequestBackground { get; set; } = "Graffiti creation request is being processed in the background.";
    }
}