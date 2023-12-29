namespace MaaranTicketingSystemAPI.Services
{
    public interface ITicketHandler
    {
        Task<string> FileOperation(int TicketId, IFormFile formFile, string Operation);

    }
}
