using System;

namespace Madingley.Common
{
    public class Stock
    {
        public double IndividualBodyMass { get; set; }

        public double TotalBiomass { get; set; }

        public Stock(
            double individualBodyMass,
            double totalBiomass)
        {
            this.IndividualBodyMass = individualBodyMass;
            this.TotalBiomass = totalBiomass;
        }

        public Stock(Stock c)
        {
            this.IndividualBodyMass = c.IndividualBodyMass;
            this.TotalBiomass = c.TotalBiomass;
        }

        public override bool Equals(Object yo)
        {
            if (yo == null) return false;

            var y = yo as Stock;
            if ((Object)y == null) return false;

            return
                this.IndividualBodyMass.Equals(y.IndividualBodyMass) &&
                this.TotalBiomass.Equals(y.TotalBiomass);
        }

        public override int GetHashCode()
        {
            return
                this.IndividualBodyMass.GetHashCode() ^
                this.TotalBiomass.GetHashCode();
        }
    }
}
