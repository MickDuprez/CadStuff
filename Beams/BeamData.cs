using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using _AcGe = Teigha.Geometry;

namespace Beams {
    /// <summary>
    /// Simple data holding class for beam entities.
    /// </summary>
    public class BeamData {

        public _AcGe.Point3d BeamStartPoint { get; private set; }
        public _AcGe.Point3d BeamEndPoint { get; private set; }
        public _AcGe.Point3d BeamSidePoint { get; private set; }
        public double Height { get; private set; }          
        public double Width { get; private set; }
        public double Length { get; private set; }
        public double MaxLength { get; private set; }
        public double BeamAngle { get; private set; }
        public double OffsetFactor { get; private set; }    // used when beam side is picked
        public string Mark { get; set; }
        

        private _AcGe.Vector3d _vecPerp, _vecLine;

        public BeamData(double height, double width, double maxlength, _AcGe.Point3d p0, _AcGe.Point3d p1, _AcGe.Point3d pSide = new _AcGe.Point3d()) {

            BeamStartPoint = p0;
            BeamEndPoint = p1;
            BeamSidePoint = pSide;
            Height = height;
            Width = width;
            Length = BeamStartPoint.GetVectorTo(BeamEndPoint).Length;
            MaxLength = maxlength;

            if(Length > MaxLength) {
                MessageBox.Show("WARNING! - picked beam length is greater than maximum length for selected beam type.");
            }

            //
            // We need to check the incoming pSide var to see if we need to offset our beam to one side
            // or draw it on the centre line (the picked points form the centre line).
            //
            if(pSide.GetAsVector().Length == 0) {
                OffsetFactor = 0.5;     // no offset so set offset factor to half width to draw on centre line
            } else {
                OffsetFactor = 1.0;     // full offset required
            }
        }
    }
}

