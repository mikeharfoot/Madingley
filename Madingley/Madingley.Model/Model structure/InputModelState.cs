using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Madingley
{
    public class InputModelState
    {

        /// <summary>
        /// The handler for the cohorts in this grid cell
        /// </summary>
        private GridCellCohortHandler[,] _GridCellCohorts;
        /// <summary>
        /// Get or set the cohorts in this grid cell
        /// </summary>
        public GridCellCohortHandler[,] GridCellCohorts
        {
            get { return _GridCellCohorts; }
            set { _GridCellCohorts = value; }
        }

        /// <summary>
        /// The handler for the stocks in this grid cell
        /// </summary>
        private GridCellStockHandler[,] _GridCellStocks;
        /// <summary>
        /// Get or set the stocks in this grid cell
        /// </summary>
        public GridCellStockHandler[,] GridCellStocks
        {
            get { return _GridCellStocks; }
            set { _GridCellStocks = value; }
        }

        private Boolean _InputState = false;

        public Boolean InputState
        {
            get { return _InputState; }
            set { _InputState = value; }
        }

#if true
        public InputModelState(
            GridCellCohortHandler[,] gridCellCohorts,
            GridCellStockHandler[,] gridCellStocks)
        {
            this._GridCellCohorts = gridCellCohorts;
            this._GridCellStocks = gridCellStocks;
            this._InputState = true;
        }
#endif
    }
}
