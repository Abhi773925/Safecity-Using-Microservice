namespace SafeCity.PFOM.HttpClients
{
    public interface IIdentityService
    {
        Task<bool> IsOfficerValidAsync(int officerId);
    }
}
