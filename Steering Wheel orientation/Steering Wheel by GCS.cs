using System;
using System.Runtime.InteropServices;
using SolidEdge.SDK;

namespace Steering_Wheel_by_GCS
{
	class Program
	{		
		[STAThread]
		public static void Main(string[] args)
		{
			SolidEdgeFramework.Application application;
            SolidEdgeFramework.SolidEdgeDocument document;
            SolidEdgeFramework.Window window;
            SolidEdgeFramework.View view;
            
            try
            { 
                // Connect to Solid Edge
                OleMessageFilter.Register();
                application = (SolidEdgeFramework.Application)Marshal.GetActiveObject("SolidEdge.Application");
                document = (SolidEdgeFramework.SolidEdgeDocument)application.ActiveDocument;
                
                if (document.Type == SolidEdgeFramework.DocumentTypeConstants.igDraftDocument) {
                	return;
                }
                
                window = (SolidEdgeFramework.Window)application.ActiveWindow;
                view = window.View;
                
            	          
                switch (document.Type)
                {
                	case SolidEdgeFramework.DocumentTypeConstants.igPartDocument:
                	case SolidEdgeFramework.DocumentTypeConstants.igSheetMetalDocument:
                		var partDocument = (SolidEdgePart.PartDocument)document;
                		//OrientSteeringWheelByView(view, partDocument.SteeringWheel);
                		OrientSteeringWheelByGCS(partDocument.SteeringWheel);
                		break;
                	case SolidEdgeFramework.DocumentTypeConstants.igAssemblyDocument:
                		var assemblyDocument = (SolidEdgeAssembly.AssemblyDocument)document;
                		//OrientSteeringWheelByView(view, assemblyDocument.SteeringWheel);
                		OrientSteeringWheelByGCS(assemblyDocument.SteeringWheel);
                		break;
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }
            finally
            {
            	application = null;
            	document = null;
            	OleMessageFilter.Revoke();
            }
		}
		
		static void OrientSteeringWheelByView(SolidEdgeFramework.View view, SolidEdgeFramework.SteeringWheel steeringWheel){
			double eyeX, eyeY, eyeZ, targetX, targetY, targetZ, upX, upY, upZ, scaleOrAngle;
			bool perspective;
			view.GetCamera(out eyeX, out eyeY, out eyeZ, out targetX, out targetY, out targetZ, out upX, out upY, out upZ, out perspective, out scaleOrAngle);
			double[] eyePoint = new double[] {eyeX,eyeY,eyeZ};
			double[] targetPoint = new double[] {targetX,targetY,targetZ};
			double[] ZAx = GetUnitVec(eyePoint, targetPoint);
			double[] YAx = {Math.Round(upX, 5), Math.Round(upY, 5), Math.Round(upZ, 5) };
			steeringWheel.Align((seSteeringWheelConstants)3, ZAx[0], ZAx[1], ZAx[2]);
			steeringWheel.Align((seSteeringWheelConstants)2, YAx[0], YAx[1], YAx[2]);
		}
		
		private static double[] GetUnitVec(double[] fromPoint, double[] toPoint){
		    double dist = Math.Sqrt((fromPoint[0] - toPoint[0]) * (fromPoint[0] - toPoint[0]) +
			(fromPoint[1] - toPoint[1]) * (fromPoint[1] - toPoint[1]) +
			(fromPoint[2] - toPoint[2]) * (fromPoint[2] - toPoint[2]));
		    return new double[] { (toPoint[0] - fromPoint[0]) / dist, (toPoint[1] - fromPoint[1]) / dist, (toPoint[2] - fromPoint[2]) / dist };
		}
	}
}
