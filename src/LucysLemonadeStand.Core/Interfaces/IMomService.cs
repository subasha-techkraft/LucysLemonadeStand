namespace LucysLemonadeStand.Core.Interfaces
{
    public interface IMomService
    {
        /// <summary>
        /// Ask more for more Pitchers
        /// </summary>
        /// <returns>cups returned</returns>
        Task<int> AskForAPitcher(float cash);

    }
}
