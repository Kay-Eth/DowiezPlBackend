namespace DowiezPlBackend.Exceptions
{
    public class UserNotFoundException : DowiezPlException
    {
        public UserNotFoundException(string name) : base($"User {name} not found.")
        {

        }

        public override int StatusCode => 400;
    }
}