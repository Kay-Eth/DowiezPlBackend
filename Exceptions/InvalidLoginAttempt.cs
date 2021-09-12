namespace DowiezPlBackend.Exceptions
{
    public class InvalidLoginAttemptException : DowiezPlException
    {
        public override int StatusCode => 400;
        public override string Message => "Login attempt failed. Cannot give more information because of security reasons.";
    }
}