namespace DowiezPlBackend.Exceptions
{
    public class BannedUserException : DowiezPlException
    {
        public override int StatusCode => 423;
        public override string Message => "This user is banned.";
    }
}