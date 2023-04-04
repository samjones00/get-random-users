namespace MyHomework.Interfaces
{
    internal interface IUserService
    {
        Task Run(CancellationToken cancellationToken);
    }
}