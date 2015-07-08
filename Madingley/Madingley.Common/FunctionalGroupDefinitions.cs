using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    /// <summary>
    /// Defines a functional group, either a cohort or a stock
    /// </summary>
    public class FunctionalGroupDefinition
    {
        /// <summary>
        /// Set of definitions
        /// </summary>
        public IDictionary<string, string> Definitions { get; set; }

        /// <summary>
        /// Set of properties
        /// </summary>
        public IDictionary<string, double> Properties { get; set; }

        /// <summary>
        /// FunctionalGroupDefinition constructor
        /// </summary>
        /// <param name="definitions">Set of definitions</param>
        /// <param name="properties">Set of properties</param>
        public FunctionalGroupDefinition(
            IDictionary<string, string> definitions,
            IDictionary<string, double> properties)
        {
            this.Definitions = new SortedList<string, string>(definitions);
            this.Properties = new SortedList<string, double>(properties);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="c"></param>
        public FunctionalGroupDefinition(FunctionalGroupDefinition c)
        {
            this.Definitions = new SortedList<string, string>(c.Definitions);
            this.Properties = new SortedList<string, double>(c.Properties);
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if objects are both FunctionalGroupDefinitions and equivalent; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null) return false;

            var y = obj as FunctionalGroupDefinition;
            if ((Object)y == null) return false;

            var ds = this.Definitions.SequenceEqual(y.Definitions, new KeyValuePairEqualityComparer<string>(EqualityComparer<String>.Default));
            var ps = this.Properties.SequenceEqual(y.Properties, new KeyValuePairEqualityComparer<double>(EqualityComparer<Double>.Default));

            return ds && ps;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return
                this.Definitions.GetHashCode() ^
                this.Properties.GetHashCode();
        }
    }

    /// <summary>
    /// A list of FunctionalGroupDefinitions, including the list of definition and property keys
    /// </summary>
    public class FunctionalGroupDefinitions
    {
        /// <summary>
        /// List of FunctionalGroupDefinitions.
        /// </summary>
        public IEnumerable<FunctionalGroupDefinition> Data { get; set; }

        /// <summary>
        /// List of definition keys.
        /// </summary>
        public IEnumerable<string> Definitions { get; set; }

        /// <summary>
        /// List of property keys.
        /// </summary>
        public IEnumerable<string> Properties { get; set; }

        /// <summary>
        /// FunctionalGroupDefinitions constructor.
        /// </summary>
        /// <param name="data">List of FunctionalGroupDefinitions.</param>
        /// <param name="definitions">List of definition keys.</param>
        /// <param name="properties">List of property keys.</param>
        public FunctionalGroupDefinitions(
            IEnumerable<FunctionalGroupDefinition> data,
            IEnumerable<string> definitions,
            IEnumerable<string> properties)
        {
            this.Data = data.Select(d => new FunctionalGroupDefinition(d)).ToArray();
            this.Definitions = definitions.OrderBy(m => m).ToArray();
            this.Properties = properties.OrderBy(m => m).ToArray();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="c">FunctionalGroupDefinitions to copy</param>
        public FunctionalGroupDefinitions(FunctionalGroupDefinitions c)
        {
            this.Data = c.Data.Select(d => new FunctionalGroupDefinition(d)).ToArray();
            this.Definitions = c.Definitions.ToArray();
            this.Properties = c.Properties.ToArray();
        }

        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>true if objects are both FunctionalGroupDefinitionss and equivalent; otherwise, false.</returns>
        public override bool Equals(Object obj)
        {
            if (obj == null) return false;

            var y = obj as FunctionalGroupDefinitions;
            if ((Object)y == null) return false;

            var ds = this.Data.SequenceEqual(y.Data);
            var dds = this.Definitions.SequenceEqual(y.Definitions);
            var pps = this.Properties.SequenceEqual(y.Properties);

            return ds && dds && pps;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return
                this.Data.GetHashCode() ^
                this.Definitions.GetHashCode() ^
                this.Properties.GetHashCode();
        }
    }
}
