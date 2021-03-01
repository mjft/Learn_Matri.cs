using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace 测方向三角网_间接平差
{
    public class Matrix
    {
        public double[] element = null;
        private int rows = 0;
        private int cols = 0;
        private double eps = 0.0;
        public int Rows
        {
            get
            {
                return rows;
            }
        }
        public int Cols
        {
            get
            {
                return cols;
            }
        }
        public double Eps
        {
            get
            {
                return eps;
            }
            set
            {
                eps = value;
            }
        }
        public double this[int i, int j]
        {
            get
            {
                if (i < Rows && j < Cols)
                {
                    return element[i * cols + j];
                }
                else
                {
                    throw new Exception("索引越界！");
                }
            }
            set
            {
                element[i * cols + j] = value;
            }
        }
        //"+"的重载
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            return m1.Add(m2);
        }

        //"-"的重载
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            return m1.Subtract(m2);
        }

        //"*"的重载
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            return m1.Multiply(m2);
        }

        public override string ToString()
        {
            return ToString(",", true);
        }

        public string ToString(string sDelim, bool bLineBreak)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rows; i++)
            {
                string s = "";
                for (int j = 0; j < cols; ++j)
                {
                    s += GetElement(i, j).ToString();
                    if (bLineBreak)
                    {
                        if (j != cols - 1)
                            s += sDelim;
                    }
                    else
                    {
                        if (i != rows - 1 || j != cols - 1)
                            s += sDelim;
                    }
                }
                sb.AppendLine(s);
            }
            return sb.ToString();
        }

        public Matrix()
        {
            cols = 1;
            rows = 1;
            Init(rows, cols);
        }

        public Matrix(Matrix other)
        {
            cols = other.GetNumColumns();
            rows = other.GetNumRows();
            Init(rows, cols);
            SetData(other.element);
        }

        public Matrix(int nRows, int nCols)
        {
            rows = nRows;
            cols = nCols;
            Init(rows, cols);
        }

        public bool Init(int nRows, int nCols)
        {
            rows = nRows;
            cols = nCols;
            int nSize = nCols * nRows;
            if (nSize < 0)
                return false;
            element = new double[nSize];
            return true;
        }

        public int GetNumColumns()
        {
            return cols;
        }

        public int GetNumRows()
        {
            return rows;
        }

        public void SetData(double[] value)
        {
            element = (double[])value.Clone();
        }

        public double GetElement(int nRow, int nCol)
        {
            return element[nCol + nRow * cols];
        }

        public bool SetElement(int nRow, int nCol, double value)
        {
            if (nCol < 0 || nCol >= cols || nRow < 0 || nRow >= rows)
                return false;
            element[nCol + nRow * cols] = value;
            return true;
        }
        public static Matrix LoadFromTextFile(string fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            if (sr == null)
            {
                return null;
            }
            ArrayList stringArray = new ArrayList();
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                stringArray.Add(line);
            }
            sr.Close();
            int rows = stringArray.Count;
            int cols = 0;
            if (stringArray.Count > 0)
            {
                String[] numberString = stringArray[0].ToString().Split(',');
                cols = numberString.Length;
            }
            else
            {
                return null;
            }
            Matrix result = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)
            {
                String[] tempData = stringArray[i].ToString().Split(',');
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = double.Parse(tempData[j]);
                }
            }
            return result;
        }
        //矩阵加法
        public Matrix Add(Matrix other)
        {
            if (cols != other.GetNumColumns() || rows != other.GetNumRows())
                throw new Exception("矩阵的行/列数不匹配。");
            Matrix result = new Matrix(this);
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                    result.SetElement(i, j, result.GetElement(i, j) + other.GetElement(i, j));

            }
            return result;
        }
        //矩阵减法
        public Matrix Subtract(Matrix other)
        {
            if (cols != other.GetNumColumns() || rows != other.GetNumRows())
                throw new Exception("矩阵的行/列数不匹配。");
            Matrix result = new Matrix(this);
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                    result.SetElement(i, j, result.GetElement(i, j) - other.GetElement(i, j));

            }

            return result;
        }
        //数乘以矩阵
        public Matrix Multiply(double value)
        {
            Matrix result = new Matrix(this);
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                    result.SetElement(i, j, result.GetElement(i, j) * value);
            }

            return result;
        }

        //矩阵乘法
        public Matrix Multiply(Matrix other)
        {
            if (cols != other.GetNumRows())
                throw new Exception("矩阵的行/列数不匹配。");
            Matrix result = new Matrix(rows, other.GetNumColumns());
            double value;
            for (int i = 0; i < result.GetNumRows(); ++i)
            {
                for (int j = 0; j < other.GetNumColumns(); ++j)
                {
                    value = 0.0;
                    for (int k = 0; k < cols; ++k)
                        value += GetElement(i, k) * other.GetElement(k, j);
                    result.SetElement(i, j, value);
                }
            }
            return result;
        }
        //矩阵转置
        public Matrix Transpose()
        {
            Matrix Trans = new Matrix(cols, rows);
            for (int i = 0; i < rows; ++i)
            {
                for (int j = 0; j < cols; ++j)
                    Trans.SetElement(j, i, GetElement(i, j));
            }
            return Trans;
        }
        //矩阵求逆
        public Matrix InvertGaussJordan()
        {
            int i, j, k, l, u, v;
            double d = 0.0, p = 0.0;
            int[] pnRow = new int[cols];
            int[] pnCol = new int[cols];
            {
                for (k = 0; k <= cols - 1; k++)
                {
                    d = 0.0;
                    for (i = k; i <= cols - 1; i++)
                    {
                        for (j = k; j <= cols - 1; j++)
                        {
                            l = i * cols + j; p = Math.Abs(element[l]);
                            if (p > d)
                            {
                                d = p;
                                pnRow[k] = i;
                                pnCol[k] = j;
                            }
                        }
                    }
                    if (d == 0.0)
                    {
                        throw new Exception("矩阵不可逆。");
                    }
                    if (pnRow[k] != k)
                    {
                        for (j = 0; j <= cols - 1; j++)
                        {
                            u = k * cols + j;
                            v = pnRow[k] * cols + j;
                            p = element[u];
                            element[u] = element[v];
                            element[v] = p;
                        }
                    }
                    if (pnCol[k] != k)
                    {
                        for (i = 0; i <= cols - 1; i++)
                        {
                            u = i * cols + k;
                            v = i * cols + pnCol[k];
                            p = element[u];
                            element[u] = element[v];
                            element[v] = p;
                        }
                    }
                    l = k * cols + k;
                    element[l] = 1.0 / element[l];
                    for (j = 0; j <= cols - 1; j++)
                    {
                        if (j != k)
                        {
                            u = k * cols + j;
                            element[u] = element[u] * element[l];
                        }
                    }
                    for (i = 0; i <= cols - 1; i++)
                    {
                        if (i != k)
                        {
                            for (j = 0; j <= cols - 1; j++)
                            {
                                if (j != k)
                                {
                                    u = i * cols + j;
                                    element[u] = element[u] - element[i * cols + k] * element[k * cols + j];
                                }
                            }
                        }
                    }
                    for (i = 0; i <= cols - 1; i++)
                    {
                        if (i != k)
                        {
                            u = i * cols + k;
                            element[u] = -element[u] * element[l];
                        }
                    }
                }
                for (k = cols - 1; k >= 0; k--)
                {
                    if (pnCol[k] != k)
                    {
                        for (j = 0; j <= cols - 1; j++)
                        {
                            u = k * cols + j;
                            v = pnCol[k] * cols + j;
                            p = element[u];
                            element[u] = element[v];
                            element[v] = p;
                        }
                    }
                    if (pnRow[k] != k)
                    {
                        for (i = 0; i <= cols - 1; i++)
                        {
                            u = i * cols + k;
                            v = i * cols + pnRow[k];
                            p = element[u];
                            element[u] = element[v];
                            element[v] = p;
                        }
                    }
                }
            }
            Matrix result = new Matrix(rows, cols);
            for (int a = 0; a < rows; ++a)
            {
                for (int b = 0; b < cols; ++b)
                {
                    result.SetElement(a, b, GetElement(a, b));
                }

            }

            return result;
        }
    }
}













