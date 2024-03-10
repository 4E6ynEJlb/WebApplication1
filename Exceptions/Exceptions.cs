using Repos;
namespace Exceptions
{
    public class DoesNotExistException : Exception
    {
        public DoesNotExistException(Type entityType)
        {
            if (entityType == typeof(Advertisement))
                Message = "Advertisement does not exists";
            else if (entityType == typeof(User))
                Message = "User does not exists";
            else if (entityType == typeof(File))
                Message = "File does not exists";
            else throw new Exception();
        }
        public new string Message;
    }
    public class InvalidPageException : Exception
    {
        public string Message = "Page number is invalid or does not exist";
    }
    public class InvalidFileFormatException : Exception
    {
        public string Message = "Sent file is not a picture";
    }
    public class EmptyFileException : Exception
    {
        public string Message = "File was not sent";
    }
    public class TooManyAdsException : Exception
    {
        public string Message = "You can not create more ads. Try to delete old one.";
    }
}
