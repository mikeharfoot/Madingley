namespace Madingley.Common
{
    public interface IGlobalProcessTracker
    {
        void RecordNPP(
            uint latIndex,
            uint lonIndex,
            uint timeStep,
            uint stock,
            double npp);

        void RecordHANPP(
            uint latIndex,
            uint lonIndex,
            uint timeStep,
            uint stock,
            double npp);
    }
}
