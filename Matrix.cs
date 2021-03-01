using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;

namespace 测方向三角网_间接平差//命名空间（使用对象）
{
    public class Matrix//类的名字
    {
        //命名属性
        
        public double[] element = null;//元素
        private int rows = 0;//row行
        private int cols = 0;//column列
        private double eps = 0.0;
        public int Rows//一个只可读的变量，get就是可读，set就是可写
        {
            get
            {
                return rows;//读取时返回rows的值
            }
        }
        public int Cols
        {
            get
            {
                return cols;
            }
        }
        public double Eps//可读也可写
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
        
        //索引器
        
        public double this[int i, int j]//因为矩阵使用二维坐标，但存储时用的是一维数组，所以需要索引器
        {
            get//使用this读取时将this显示的二维坐标转化为储存时使用的一维，并添加越界提醒功能
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
            set//写入this时同样将this显示的二维坐标转化为储存时使用的一维
            {
                element[i * cols + j] = value;
            }
        }
        //"+"的重载，"+"本身是十进制运算，重载为矩阵运算，operator
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

        //重写Tostring()
        public override string ToString()
        {
            return ToString(",", true);
        }

        public string ToString(string sDelim, bool bLineBreak)
        {
            StringBuilder sb = new StringBuilder();//StringBuilder是可变的字符串
            for (int i = 0; i < rows; i++)
            {
                string s = "";
                for (int j = 0; j < cols; ++j)
                {
                    s += GetElement(i, j).ToString();//不懂，重写的ToString()返回时用的是本函数ToString(,)，这不是无限循环了吗
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
                sb.AppendLine(s);//AppendLine是回车后添加一行，Append是不回车
            }
            return sb.ToString();//StringBuilder转String
        }

        public Matrix()//用于初始化矩阵，即Matrix xxx = new Matrix()
        {
            cols = 1;
            rows = 1;
            Init(rows, cols);//初始化element数组（大小）的函数，代码在下方
        }

        public Matrix(Matrix other)//用于获取进行运算的other矩阵的行列数，从而建立结果矩阵，即Matrix Result = New Matrix(this)//this继承无参时的构造函数
        {
            cols = other.GetNumColumns();
            rows = other.GetNumRows();
            Init(rows, cols);
            SetData(other.element);//使用深复制的原因？
        }

        public Matrix(int nRows, int nCols)//用于初始化矩阵
        {
            rows = nRows;
            cols = nCols;
            Init(rows, cols);
        }

        public bool Init(int nRows, int nCols)//通过行列数计算元素总数，然后初始化element数组
        {
            rows = nRows;
            cols = nCols;
            int nSize = nCols * nRows;
            if (nSize < 0)
                return false;
            element = new double[nSize];
            return true;
        }

        public int GetNumColumns()//用于获取列数的变量，只读
        {
            return cols;
        }

        public int GetNumRows()//同上
        {
            return rows;
        }

        public void SetData(double[] value)
        {
            element = (double[])value.Clone();//Clone是深复制，直接赋值是如a = b，a的值和b一样，虽然a不同于b的变量，但两者是公用同一内存的，类似指针
                                              //使用深复制，a = b.Clone()，这时两者在值上相等，并使用不同内存，是完全独立的变量
        }

        public double GetElement(int nRow, int nCol)//同为索引器，为何不用上面功能更全的this索引器？
        {
            return element[nCol + nRow * cols];
        }

        public bool SetElement(int nRow, int nCol, double value)//修改单个元素，带越界保护
        {
            if (nCol < 0 || nCol >= cols || nRow < 0 || nRow >= rows)
                return false;
            element[nCol + nRow * cols] = value;
            return true;
        }
        public static Matrix LoadFromTextFile(string fileName)//从文件导入矩阵（也可先把string转成文件流再导入矩阵）
            //转换代码如下
            /*
            string X0 = {1,2,3,\n,4,5,6,\n,7,8,9};
            using (StreamWriter obi = new StreamWriter("X0"))
            {
                obi.Write(X0);
            }
            Matrix Matrix_X0 = new Matrix();
            Matrix_X0 = Matrix.LoadFromTextFile("X0");*/
        {
            StreamReader sr = new StreamReader(fileName);
            if (sr == null)
            {
                return null;
            }
            ArrayList stringArray = new ArrayList();//ArrayList是长度可变的数组，可存放任意类型的数据
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                stringArray.Add(line);//一列为一个元素
            }
            sr.Close();
            int rows = stringArray.Count;//记录行数
            int cols = 0;
            if (stringArray.Count > 0)
            {
                String[] numberString = stringArray[0].ToString().Split(',');//读取第一行数据
                cols = numberString.Length;//记录列数
            }
            else
            {
                return null;
            }
            Matrix result = new Matrix(rows, cols);
            for (int i = 0; i < rows; i++)//录入为矩阵形式
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













