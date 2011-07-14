namespace Palaver.Interfaces
{
    /// <summary>
    /// Every gateway (class the application will access the data layer through) should implement
    /// this interface.
    /// 
    /// If the application depends only on IGateway and IFacade, any number of new gateways can be created and 
    /// used interchangeably. eg. different gateways for EF, NHibernate, special behavior for mocking, etc.
    /// </summary>
    public interface IGateway
    {
        IFacade Helpers { get; set; }
    }
}