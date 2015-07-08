using System;

namespace Madingley.Common
{
    /// <summary>
    /// Hold individual stocks
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// Mean body mass of an individual in this stock.
        /// </summary>
        public double IndividualBodyMass { get; set; }

        /// <summary>
        /// Total biomass of the stock.
        /// </summary>
        public double TotalBiomass { get; set; }

        /// <summary>
        /// Stock constructor
        /// </summary>
        /// <param name="individualBodyMass">Mean body mass of an individual</param>
        /// <param name="totalBiomass">Total biomass</param>
        public Stock(
            double individualBodyMass,
            double totalBiomass)
        {
            this.IndividualBodyMass = individualBodyMass;
            this.TotalBiomass = totalBiomass;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="stock"></param>
        public Stock(Stock stock)
        {
            this.IndividualBodyMass = stock.IndividualBodyMass;
            this.TotalBiomass = stock.TotalBiomass;
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if objects are both Stocks and equivalent; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null) return false;

            var y = obj as Stock;
            if ((Object)y == null) return false;

            return
                this.IndividualBodyMass.Equals(y.IndividualBodyMass) &&
                this.TotalBiomass.Equals(y.TotalBiomass);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return
                this.IndividualBodyMass.GetHashCode() ^
                this.TotalBiomass.GetHashCode();
        }
    }
}
