using System;

namespace Madingley.Common
{
    /// <summary>
    /// Hold individual stocks
    /// </summary>
    public class Stock
    {
        /// <summary>
        /// Index of the functional group the stock belongs to.
        /// </summary>
        public int FunctionalGroupIndex { get; set; }

        /// <summary>
        /// Mean body mass of an individual in this stock.
        /// </summary>
        public double IndividualBodyMass { get; set; }

        /// <summary>
        /// Total biomass of the stock.
        /// </summary>
        public double TotalBiomass { get; set; }

        /// <summary>
        /// Stock default constructor
        /// </summary>
        public Stock()
        {
            this.FunctionalGroupIndex = 0;
            this.IndividualBodyMass = 0.0;
            this.TotalBiomass = 0.0;
        }

        /// <summary>
        /// Stock constructor.
        /// </summary>
        /// <param name="functionalGroupIndex">Index of the functional group the stock belongs to.</param>
        /// <param name="individualBodyMass">Mean body mass of an individual.</param>
        /// <param name="totalBiomass">Total biomass.</param>
        public Stock(
            int functionalGroupIndex,
            double individualBodyMass,
            double totalBiomass)
        {
            this.FunctionalGroupIndex = functionalGroupIndex;
            this.IndividualBodyMass = individualBodyMass;
            this.TotalBiomass = totalBiomass;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        /// <param name="stock">Stock to copy.</param>
        public Stock(Stock stock)
        {
            this.FunctionalGroupIndex = stock.FunctionalGroupIndex;
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

            var stockObj = obj as Stock;
            if ((Object)stockObj == null) return false;

            return
                this.FunctionalGroupIndex.Equals(stockObj.FunctionalGroupIndex) &&
                this.IndividualBodyMass.Equals(stockObj.IndividualBodyMass) &&
                this.TotalBiomass.Equals(stockObj.TotalBiomass);
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return
                this.FunctionalGroupIndex.GetHashCode() ^
                this.IndividualBodyMass.GetHashCode() ^
                this.TotalBiomass.GetHashCode();
        }
    }
}
