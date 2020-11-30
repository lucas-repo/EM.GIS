using EM.GIS.Geometries;

namespace EM.GIS.Data
{
    /// <summary>
    /// The AffineTransform class represents a 2D affine transform that performs a linear mapping from 2D coordinates to other 2D coordinates that preserves
    /// the "straightness" and "parallelness" of lines. Affine transformations can be constructed using sequences of translations, scales, flips, rotations, and shears.
    /// </summary>
    public class AffineTransform
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AffineTransform"/> class with the 6 specifiable entries of the 3x3 transformation matrix.
        /// </summary>
        /// <param name="inCoefficients">An array with the 6 specifiable entries of the 3x3 transformation matrix.</param>
        public AffineTransform(double[] inCoefficients)
        {
            Coefficients = inCoefficients;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the array of coefficients for this transform.
        /// </summary>
        public double[] Coefficients { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Given any input row or column, this returns the appropriate geographic location for the
        /// position of the center of the cell.
        /// </summary>
        /// <param name="row">The integer row index from 0 to numRows - 1</param>
        /// <param name="column">The integer column index from 0 to numColumns - 1</param>
        /// <returns>The geographic position of the center of the specified cell</returns>
        public ICoordinate CellCenterToProj(int row, int column)
        {
            double x = Coefficients[0] + Coefficients[1] * column + Coefficients[2] * row;
            double y = Coefficients[3] + Coefficients[4] * column + Coefficients[5] * row;
            return new Coordinate(x, y);
        }

        /// <summary>
        /// Given the row and column, this returns the geographic position of the top left corner of the cell
        /// </summary>
        /// <param name="row">The integer row index from 0 to numRows - 1</param>
        /// <param name="column">The integer column index from 0 to numColumns - 1</param>
        /// <returns>The geographic position of the top left corner of the specified cell</returns>
        public ICoordinate CellTopLeftToProj(int row, int column)
        {
            double x = Coefficients[0] + Coefficients[1] * (column - 0.5) + Coefficients[2] * (row - 0.5);
            double y = Coefficients[3] + Coefficients[4] * (column - 0.5) + Coefficients[5] * (row - 0.5);
            return new Coordinate(x, y);
        }

        /// <summary>
        /// Given the row and column, this returns the geographic position of the top right corner of the cell
        /// </summary>
        /// <param name="row">The integer row index from 0 to numRows - 1</param>
        /// <param name="column">The integer column index from 0 to numColumns - 1</param>
        /// <returns>The geographic position of the top right corner of the specified cell</returns>
        public ICoordinate CellTopRightToProj(int row, int column)
        {
            double x = Coefficients[0] + Coefficients[1] * (column + 0.5) + Coefficients[2] * (row - 0.5);
            double y = Coefficients[3] + Coefficients[4] * (column + 0.5) + Coefficients[5] * (row - 0.5);
            return new Coordinate(x, y);
        }

        /// <summary>
        /// Given the row and column, this returns the geographic position of the bottom left corner of the cell
        /// </summary>
        /// <param name="row">The integer row index from 0 to numRows - 1</param>
        /// <param name="column">The integer column index from 0 to numColumns - 1</param>
        /// <returns>The geographic position of the bottom left corner of the specified cell</returns>
        public ICoordinate CellBottomLeftToProj(int row, int column)
        {
            double x = Coefficients[0] + Coefficients[1] * (column - 0.5) + Coefficients[2] * (row + 0.5);
            double y = Coefficients[3] + Coefficients[4] * (column - 0.5) + Coefficients[5] * (row + 0.5);
            return new Coordinate(x, y);
        }

        /// <summary>
        /// Given the row and column, this returns the geographic position of the bottom right corner of the cell
        /// </summary>
        /// <param name="row">The integer row index from 0 to numRows - 1</param>
        /// <param name="column">The integer column index from 0 to numColumns - 1</param>
        /// <returns>The geographic position of the bottom right corner of the specified cell</returns>
        public ICoordinate CellBottomRightToProj(int row, int column)
        {
            double x = Coefficients[0] + Coefficients[1] * (column + 0.5) + Coefficients[2] * (row + 0.5);
            double y = Coefficients[3] + Coefficients[4] * (column + 0.5) + Coefficients[5] * (row + 0.5);
            return new Coordinate(x, y);
        }

        /// <summary>
        ///  Given the row and column, this returns new affine coefficients transformed to that cell
        /// </summary>
        /// <param name="startColumn">The column index from 0 to numColumns - 1</param>
        /// <param name="startRow">The row index from 0 to numRows - 1</param>
        /// <returns>Transformed affine coefficients</returns>
        public double[] TransfromToCorner(double startColumn, double startRow)
        {
            // X = [0] + [1] * column + [2] * row;
            // Y = [3] + [4] * column + [5] * row;
            var ba = new double[6];
            ba[0] = Coefficients[0] + Coefficients[1] * startColumn + Coefficients[2] * startRow;
            ba[1] = Coefficients[1];
            ba[2] = Coefficients[2];
            ba[3] = Coefficients[3] + Coefficients[4] * startColumn + Coefficients[5] * startRow;
            ba[4] = Coefficients[4];
            ba[5] = Coefficients[5];
            return ba;
        }

        #endregion

    }
}