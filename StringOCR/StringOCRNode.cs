#region usings
using System;
using System.ComponentModel.Composition;

using VVVV.PluginInterfaces.V1;
using VVVV.PluginInterfaces.V2;
using VVVV.Utils.VColor;
using VVVV.Utils.VMath;

using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System.Xml;


using VVVV.Core.Logging;
#endregion usings

namespace VVVV.Nodes
{
	#region PluginInfo
	[PluginInfo(Name = "OCR", Category = "String", Help = "Basic template with one string in/out", Tags = "")]
	#endregion PluginInfo
	public class StringOCRNode : IPluginEvaluate
	{
		#region fields & pins
		[Input("Image Path", DefaultString = "path",StringType = StringType.Filename)]
		public ISpread<string> FInput;
		
		[Input("Tessdata Folder",StringType = StringType.Directory)]
		public ISpread<string> FTessdata;
		
		[Input("OCR")]
		public IDiffSpread<bool> FInit;

		[Output("Output")]
		public ISpread<string> FOutput;

		 private Tesseract _ocr;
		
		[Import()]
		public ILogger FLogger;
		#endregion fields & pins

		//called when data for any output pin is requested
		public void Evaluate(int SpreadMax)
		{
			if (_ocr == null)
			{
				try
				{
					_ocr = new Tesseract(@FTessdata[0], "eng", Tesseract.OcrEngineMode.OEM_TESSERACT_ONLY);
				}
				catch (Exception exception){
					
					FLogger.Log(LogType.Debug, exception.Message);
				}	
			}
			
			
			if (FInit.IsChanged && FInit[0] == true)
			{
				
				Image<Bgr, byte> My_Image = new Image<Bgr, byte>(@FInput[0]);
				Image<Gray, byte> gray = My_Image.Convert<Gray, Byte>();
				_ocr.Recognize(gray);
				FOutput[0] = _ocr.GetText();
			}
			
			
			//FOutput[0] = "hallo";
		}
	}
}
