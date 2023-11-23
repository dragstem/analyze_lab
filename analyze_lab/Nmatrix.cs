using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;

namespace SoLE
{
    public class Nmatrix
    {
        public int Rows { get; }
        public int Columns { get; }
        private double[,] data;

        public Nmatrix(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            data = new double[rows, columns];
        }

        public Nmatrix(int rows, int columns, int upperBound)
        {
            Rows = rows;
            Columns = columns;
            data = new double[rows, columns];
            var random = new Random();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    data[i, j] = random.NextDouble() * upperBound;
                }
            }
        }

        public double this[int i, int j]
        {
            get { return data[i, j]; }
            set { data[i, j] = value; }
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Columns; j++)
                {
                    builder.AppendFormat("{0:f2}\t", data[i, j]);
                }
                builder.AppendLine();
            }
            return builder.ToString();
        }

        public static Nmatrix operator *(Nmatrix a, Nmatrix b)
        {
            if (a.Columns != b.Rows)
                throw new ArgumentException("Матрицы не могут быть перемножены. Количество столбцов первой матрицы должно быть равно количеству строк второй матрицы.");

            var result = new Nmatrix(a.Rows, b.Columns);
            for (int i = 0; i < result.Rows; i++)
            {
                for (int j = 0; j < result.Columns; j++)
                {
                    for (int k = 0; k < a.Columns; k++)
                    {
                        result[i, j] += a[i, k] * b[k, j];
                    }
                }
            }
            return result;
        }

        public static Nmatrix operator +(Nmatrix a, Nmatrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new ArgumentException("Матрицы не могут быть сложены. Размеры матриц должны быть одинаковыми.");

            var result = new Nmatrix(a.Rows, a.Columns);
            for (int i = 0; i < result.Rows; i++)
            {
                for (int j = 0; j < result.Columns; j++)
                {
                    result[i, j] = a[i, j] + b[i, j];
                }
            }
            return result;
        }

        public static Nmatrix operator -(Nmatrix a, Nmatrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                throw new ArgumentException("Матрицы не могут быть вычтены. Размеры матриц должны быть одинаковыми.");

            var result = new Nmatrix(a.Rows, a.Columns);
            for (int i = 0; i < result.Rows; i++)
            {
                for (int j = 0; j < result.Columns; j++)
                {
                    result[i, j] = a[i, j] - b[i, j];
                }
            }
            return result;
        }

        public void ToTxt(string filename)
        {
            using (var writer = new StreamWriter(filename))
            {
                for (int i = 0; i < Rows; i++)
                {
                    for (int j = 0; j < Columns; j++)
                    {
                        writer.Write($"{data[i, j]:f2} ");
                    }
                    writer.WriteLine();
                }
            }
        }
        public static bool operator ==(Nmatrix a, Nmatrix b)
        {
            if (a.Rows != b.Rows || a.Columns != b.Columns)
                return false;

            for (int i = 0; i < a.Rows; i++)
            {
                for (int j = 0; j < a.Columns; j++)
                {
                    if (Math.Round(a[i, j], 2) != Math.Round(b[i, j], 2))
                        return false;
                }
            }
            return true;
        }

        public static bool operator !=(Nmatrix a, Nmatrix b)
        {
            return !(a == b);
        }



        public static Nmatrix operator %(Nmatrix a, Nmatrix b)
        {
            if (a.Rows != a.Columns || b.Rows != b.Columns || a.Rows != b.Rows)
                throw new ArgumentException("Матрицы должны быть квадратными и одного размера для умножения методом Штрассена.");

            if (a.Rows <= 63)
                return a * b;

            int size = a.Rows / 2;
            var a11 = a.Submatrix(0, 0, size);
            var a12 = a.Submatrix(0, size, size);
            var a21 = a.Submatrix(size, 0, size);
            var a22 = a.Submatrix(size, size, size);

            var b11 = b.Submatrix(0, 0, size);
            var b12 = b.Submatrix(0, size, size);
            var b21 = b.Submatrix(size, 0, size);
            var b22 = b.Submatrix(size, size, size);

            var p1 = (a11 + a22) % (b11 + b22);
            var p2 = (a21 + a22) % b11;
            var p3 = a11 % (b12 - b22);
            var p4 = a22 % (b21 - b11);
            var p5 = (a11 + a12) % b22;
            var p6 = (a21 - a11) % (b11 + b12);
            var p7 = (a12 - a22) % (b21 + b22);

            var c11 = p1 + p4 - p5 + p7;
            var c12 = p3 + p5;
            var c21 = p2 + p4;
            var c22 = p1 - p2 + p3 + p6;

            return Combine(c11, c12, c21, c22);
        }

        private Nmatrix Submatrix(int row, int col, int size)
        {
            var result = new Nmatrix(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    result[i, j] = this[row + i, col + j];
                }
            }
            return result;
        }

        private static Nmatrix Combine(Nmatrix c11, Nmatrix c12, Nmatrix c21, Nmatrix c22)
        {
            int size = c11.Rows * 2;
            var result = new Nmatrix(size, size);
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (i < size / 2)
                    {
                        if (j < size / 2)
                            result[i, j] = c11[i, j];
                        else
                            result[i, j] = c12[i, j - size / 2];
                    }
                    else
                    {
                        if (j < size / 2)
                            result[i, j] = c21[i - size / 2, j];
                        else
                            result[i, j] = c22[i - size / 2, j - size / 2];
                    }
                }
            }
            return result;
        }

    }
}