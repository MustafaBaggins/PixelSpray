using Exiled.API.Interfaces;


namespace PixelSpray.Configuration
{
    public class Translation : ITranslation
    {
        public string SprayCommandDescription { get; set; } = "Creates a Spray in-game from the image at the specified URL.";
        public string NoPermission { get; set; } = "You do not have the required permission to use this command.";
        public string Usage { get; set; } = "Wrong Syntax, use ~pixelspray help~";
        public string RequestReceived { get; set; } = "Your Spray request has been received ({0}), the image is being processed... Please wait.";
        public string SprayEmptyResult { get; set; } = "Spray could not be created (empty result).";
        public string SprayCommandSent { get; set; } = "Spray command sent to the server. Check if it has been created.";
        public string SprayCommandError { get; set; } = "An error occurred while creating Spray (command execution stage).";
        public string ImageDownloadError { get; set; } = "ERROR: Image could not be downloaded. (Network Error: {0})";
        public string SprayGeneralError { get; set; } = "ERROR: There was a problem creating the Spray.";
        public string LabelNotFound { get; set; } = "ERROR: The label you provided does not exist or is invalid.";
        public string IdNotFound { get; set; } = "ERROR: The ID you provided does not exist or is invalid.";
        public string SprayRequestBackground { get; set; } = "Spray creation request is being processed in the background.";
        public string RemovedSprays { get; set; } = "Removed {0} Spray(s).";
        public string SprayListHeader { get; set; } = "Currently spawned Spray:";
        public string SprayInformation { get; set; } = "Spray ID: {0}, Spray Label: {1}, Owner: {2}";
    }
}