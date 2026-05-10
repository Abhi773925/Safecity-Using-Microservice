namespace SafeCity.DCR.HttpClients
{
    public interface IIdentityService
    {
        Task<bool> IsPoliceOfficerAsync(int userId);
    }
}
