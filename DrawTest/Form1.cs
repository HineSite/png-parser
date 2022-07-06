using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using PngDecoder;

namespace DrawTest
{
   public partial class Form1 : Form
   {
      List<Chunk> _chunks;
      int offset = 0;

      public Form1()
      {
         InitializeComponent();
      }

      private void Form1_Load(object sender, EventArgs e)
      {
         Decoder decoder = new Decoder();
         _chunks = decoder.Decode(@"test1.png");
      }

      private void Form1_Paint(object sender, PaintEventArgs e)
      {
         Graphics g = e.Graphics;
         offsetLabel.Text = offset.ToString();
         
         if (_chunks == null)
            return;

         int byteCount = 0;
         int columnCount = 0;
         int rowCount = 0;
         byte[] colorBytes = new byte[3];
         bool breakIt = false;
         uint width = 0;
         uint height = 0;

         List<byte> compressedBytes = new List<byte>();
         byte[] data = null;
         bool first = true;

         foreach (Chunk chunk in _chunks)
         {
            if (chunk.Type.GetType() != typeof(PngDecoder.Types.IDAT))
            {
               if (chunk.Type.GetType() == typeof(PngDecoder.Types.IHDR))
               {
                  width = ((PngDecoder.Types.IHDR)chunk.Type).Width;
                  height = ((PngDecoder.Types.IHDR)chunk.Type).Height;
               }

               continue;
            }

            if (first)
            {
               first = false;
               compressedBytes.AddRange(chunk.Type.getData().Skip(2));
            }
            else
            {
               compressedBytes.AddRange(chunk.Type.getData());
            }
         }

         using (MemoryStream memoryStream = new MemoryStream(compressedBytes.ToArray()))
         {
            using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress, false))
            {
               int bite;
               List<byte> bytes = new List<byte>();

               while ((bite = deflateStream.ReadByte()) != -1)
                  bytes.Add((byte)bite);

               data = bytes.ToArray();
            }
         }

         int bytesPerLine = (int)(data.Length / height);
         List<byte[]> imageDat = new List<byte[]>();
         for (int i = 0; i < height; i++)
         {
            imageDat.Add(data.Skip((int)(i * bytesPerLine)).Take((int)bytesPerLine).ToArray());
         }

         byte[] defiltered = null;
         foreach (byte[] scanLine in imageDat)
         {
            Console.WriteLine(scanLine[0]);
            defiltered = defilter(scanLine, defiltered);

            for (int i = 0; i < defiltered.Length; i++)
            {
               colorBytes[byteCount] = defiltered[i];

               byteCount++;
               if (byteCount == 3)
               {
                  byteCount = 0;

                  Color color = Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2]);
                  Brush brush = new SolidBrush(color);
                  g.FillRectangle(brush, columnCount, rowCount, 1, 1);

                  columnCount++;
                  if (columnCount == width)
                     columnCount = 0;
               }
            }

            rowCount++;
         }

         /*for (int i = 0; i < data.Length; i++)
         {
            // Ignore the filter
            if (byteCount == 0 && columnCount == 0)
               i++;

            colorBytes[byteCount] = data[i];

            byteCount++;
            if (byteCount == 3)
            {
               byteCount = 0;

               Color color = Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2]);
               Brush brush = new SolidBrush(color);
               g.FillRectangle(brush, columnCount, rowCount + 0, 1, 1);

               columnCount++;
               if (columnCount == width)
               {
                  columnCount = 0;

                  rowCount++;
                  if (rowCount == 300)
                  {
                     //breakIt = true;
                     //break;
                  }
               }
            }
         }*/

         /*foreach (Chunk chunk in _chunks)
         {
            if (chunk.Type.GetType() != typeof(PngDecoder.Types.IDAT))
            {
               if (chunk.Type.GetType() == typeof(PngDecoder.Types.IHDR))
               {
                  width = ((PngDecoder.Types.IHDR)chunk.Type).Width;
               }

               continue;
            }

            byte[] data = null;// chunk.Type.getData();
            List<byte> bytes = new List<byte>();



            using (MemoryStream memoryStream = new MemoryStream(chunk.Type.getData()))
            {
               using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress, false))
               {
                  int bite;
                  while ((bite = deflateStream.ReadByte()) != -1)
                     bytes.Add((byte)bite);

                  data = bytes.ToArray();
               }
            }

            //foreach (byte bite in chunk.Type.getData())
            for (int i = offset; i < data.Length; i++)
            {
               colorBytes[byteCount] = data[i];

               byteCount++;
               if (byteCount == 3)
               {
                  byteCount = 0;

                  Color color = Color.FromArgb(colorBytes[0], colorBytes[1], colorBytes[2]);
                  Brush brush = new SolidBrush(color);
                  g.FillRectangle(brush, columnCount, rowCount + 0, 1, 1);

                  columnCount++;
                  if (columnCount == width)
                  {
                     columnCount = 0;

                     rowCount++;
                     if (rowCount == 300)
                     {
                        //breakIt = true;
                        //break;
                     }
                  }
               }
            }

            if (breakIt)
               break;
         }*/

         offset++;
         //System.Threading.Thread.Sleep(200);
         //base.Refresh();
      }

      private byte[] defilter(byte[] scanLine, byte[] previousDefilteredScanLine)
      {
         byte filter = scanLine[0];
         byte[] dest = new byte[scanLine.Length - 1];
         byte bytesPerPixle = 3;
         
         if (/*strictMode && */previousDefilteredScanLine == null && filter > 1)
            filter = 1; // Assuming 0 is unlikely...

         // First byte of the scan line starts at 1...

         switch (filter)
         {
            case 1:
            {
               for (int i = (bytesPerPixle + 1); i < scanLine.Length; i++)
               {
                  scanLine[i] += scanLine[i - bytesPerPixle];
               }

               break;
            }

            case 2:
            {
               for (int i = 1; i < scanLine.Length; i++)
                  scanLine[i] += previousDefilteredScanLine[i - 1];

               break;
            }

            case 3:
            {
               for (int i = (bytesPerPixle + 1); i < scanLine.Length; i++)
                  scanLine[i] += (byte)((scanLine[i - bytesPerPixle] + previousDefilteredScanLine[i - 1]) / 2);

               break;
            }

            case 4:
            {
                  for (int i = (bytesPerPixle + 1); i < scanLine.Length; i++)
                     scanLine[i] += PaethPredictor(scanLine[i - bytesPerPixle], previousDefilteredScanLine[i - 1], previousDefilteredScanLine[i - (bytesPerPixle + 1)]);

                  break;
            }
         }

         Array.Copy(scanLine, 1, dest, 0, dest.Length);

         return dest;
      }

      public byte PaethPredictor(byte left, byte above, byte aboveLeft)
      {
         int p = (int)((left + above) - aboveLeft); // initial estimate
         int pLeft = (int)Math.Abs(p - left); // distances to left
         int pAbove = (int)Math.Abs(p - above); // distances to above
         int pAboveLeft = (int)Math.Abs(p - aboveLeft); // distances to aboveLeft

         // return nearest
         // breaking ties in order left, above, aboveLeft.

         if (pLeft <= pAbove && pLeft <= pAboveLeft)
            return left;

         if (pAbove <= pAboveLeft)
            return above;

         return aboveLeft;
      }
   }
}
