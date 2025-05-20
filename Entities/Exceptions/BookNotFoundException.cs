namespace Entities.Exceptions
{
    public abstract partial class NotFound
    {
        public sealed class BookNotFoundException : NotFoundException
        {
            public BookNotFoundException(int id)
                : base($"The book with:{id} could not found.")
            {
            }
        }
    }
}
