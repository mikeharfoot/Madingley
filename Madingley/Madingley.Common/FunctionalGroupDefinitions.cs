using System;
using System.Collections.Generic;
using System.Linq;

namespace Madingley.Common
{
    public class FunctionalGroupDefinition
    {
        public IDictionary<string, string> Definitions { get; set; }

        public IDictionary<string, double> Properties { get; set; }

        public FunctionalGroupDefinition(
            IDictionary<string, string> definitions,
            IDictionary<string, double> properties)
        {
            this.Definitions = new SortedList<string, string>(definitions);
            this.Properties = new SortedList<string, double>(properties);
        }

        public FunctionalGroupDefinition(FunctionalGroupDefinition c)
        {
            this.Definitions = new SortedList<string, string>(c.Definitions);
            this.Properties = new SortedList<string, double>(c.Properties);
        }

        public override bool Equals(Object yo)
        {
            if (yo == null) return false;

            var y = yo as FunctionalGroupDefinition;
            if ((Object)y == null) return false;

            var ds = this.Definitions.SequenceEqual(y.Definitions, new KeyValuePairEqualityComparer<string>(EqualityComparer<String>.Default));
            var ps = this.Properties.SequenceEqual(y.Properties, new KeyValuePairEqualityComparer<double>(new TolerantDoubleComparer(1L)));

            return ds && ps;
        }

        public override int GetHashCode()
        {
            return
                this.Definitions.GetHashCode() ^
                this.Properties.GetHashCode();
        }
    }

    public class FunctionalGroupDefinitions
    {
        public IEnumerable<FunctionalGroupDefinition> Data { get; set; }

        public IEnumerable<string> Definitions { get; set; }

        public IEnumerable<string> Properties { get; set; }

        public FunctionalGroupDefinitions(
            IEnumerable<FunctionalGroupDefinition> data,
            IEnumerable<string> definitions,
            IEnumerable<string> properties)
        {
            this.Data = data.Select(d => new FunctionalGroupDefinition(d)).ToArray();
            this.Definitions = definitions.OrderBy(m => m).ToArray();
            this.Properties = properties.OrderBy(m => m).ToArray();
        }

        public FunctionalGroupDefinitions(FunctionalGroupDefinitions c)
        {
            this.Data = c.Data.Select(d => new FunctionalGroupDefinition(d)).ToArray();
            this.Definitions = c.Definitions.ToArray();
            this.Properties = c.Properties.ToArray();
        }

        public override bool Equals(Object yo)
        {
            if (yo == null) return false;

            var y = yo as FunctionalGroupDefinitions;
            if ((Object)y == null) return false;

            var ds = this.Data.SequenceEqual(y.Data);
            var dds = this.Definitions.SequenceEqual(y.Definitions);
            var pps = this.Properties.SequenceEqual(y.Properties);

            return ds && dds && pps;
        }

        public override int GetHashCode()
        {
            return
                this.Data.GetHashCode() ^
                this.Definitions.GetHashCode() ^
                this.Properties.GetHashCode();
        }
    }
}
