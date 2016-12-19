using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using _AcAp = Bricscad.Runtime;
using _AcDb = Teigha.DatabaseServices;
using _AcGe = Teigha.Geometry;

namespace Beams {
    /// <summary>
    /// Helper class to do all drawing tasks for different profiles/shapes etc.
    /// </summary>
    class BeamDrawer{
        private BeamData _beamData;
        private _AcGe.Point3d _p0, _p1, _p2, _p3;
        private _AcGe.Vector3d _vecPerp, _vecLine;


        public BeamDrawer(BeamData beamData) {
            _beamData = beamData;
        }


        public void DrawBeam() {
            //
            // capture the object ids of things as they are created, this is handy if we need to edit
            // them later on (such as set layer etc)
            //
            _AcDb.ObjectId beamId;

            CalculateBeamPoints();
            beamId = DrawBeamOutline();

            //
            // Call draw text and hatch etc.
            //
        }


        private _AcDb.ObjectId DrawBeamOutline() {
            _AcDb.ObjectId id;

            var db = _AcDb.HostApplicationServices.WorkingDatabase;
            using (var tr = db.TransactionManager.StartTransaction()) {
                var btr = tr.GetObject(db.CurrentSpaceId, _AcDb.OpenMode.ForWrite) as _AcDb.BlockTableRecord;

                _AcDb.Face face = new _AcDb.Face(_p0, _p1, _p2, _p3, true, true, true, true);
                face.ColorIndex = 4;

                id = btr.AppendEntity(face);
                tr.AddNewlyCreatedDBObject(face, true);
                tr.Commit();
            }
            return id;
        }


        private void CalculateBeamPoints() {
            //
            // Calculate the vector perpendicular to the picked points. 
            // Note: we are assuming all work is done in the 'world' coordinate system, i.e. 2D
            //
            _vecLine = _beamData.BeamStartPoint.GetVectorTo(_beamData.BeamEndPoint);
            _vecPerp = _vecLine.RotateBy((Math.PI / 2), _AcGe.Vector3d.ZAxis);

            //
            // Calculate the intersection of the beam side point as a line parallel to the
            // beam line and the perp vector, this will give us the start corner.
            //
            _AcGe.Line3d sideLine = new _AcGe.Line3d(_beamData.BeamStartPoint, _beamData.BeamEndPoint);
            // move it to the side picked point:
            sideLine.TransformBy(_AcGe.Matrix3d.Displacement(_beamData.BeamStartPoint.GetVectorTo(_beamData.BeamSidePoint)));
            _AcGe.Line3d perpline = new _AcGe.Line3d(_beamData.BeamStartPoint, _vecPerp);
            _AcGe.Point3d perpPoint = perpline.IntersectWith(sideLine)[0];

            //
            // Now we need to get the normalised vector to the perpPoint, we then scale it to the beam width
            // and use it to calculate all other perp points.
            //
            _vecPerp = _beamData.BeamStartPoint.GetVectorTo(perpPoint).GetNormal();
            _vecPerp = _vecPerp * (_beamData.Width * _beamData.OffsetFactor);


            if (_beamData.OffsetFactor == 1.0) {
                _p0 = _beamData.BeamStartPoint.TransformBy(_AcGe.Matrix3d.Displacement(_vecPerp));
                _p1 = _beamData.BeamStartPoint;
                _p2 = _beamData.BeamEndPoint;
                _p3 = _beamData.BeamEndPoint.TransformBy(_AcGe.Matrix3d.Displacement(_vecPerp));
            } else {
                _p0 = _beamData.BeamStartPoint.TransformBy(_AcGe.Matrix3d.Displacement(_vecPerp));
                _p1 = _beamData.BeamStartPoint.TransformBy(_AcGe.Matrix3d.Displacement(_vecPerp.Negate()));
                _p2 = _beamData.BeamEndPoint.TransformBy(_AcGe.Matrix3d.Displacement(_vecPerp.Negate()));
                _p3 = _beamData.BeamEndPoint.TransformBy(_AcGe.Matrix3d.Displacement(_vecPerp));
            }
        }

    }
}
