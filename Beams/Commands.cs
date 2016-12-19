using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using _AcAp = Bricscad.ApplicationServices;
using _AcDb = Teigha.DatabaseServices;
using _AcEd = Bricscad.EditorInput;
using _AcGe = Teigha.Geometry;
using _AcRx = Teigha.Runtime;


// set up the entry point:
[assembly: _AcRx.ExtensionApplication(typeof(Beams.Commands))]
// let cad know what our commands class name is:
[assembly: _AcRx.CommandClass(typeof(Beams.Commands))]

namespace Beams {

    public class Commands : _AcRx.IExtensionApplication {

        public void Initialize() {
            _AcEd.Editor ed = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;

            string assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ed.WriteMessage("\nBeams application version:{0} loaded successfully!", assemblyVersion);
            ed.WriteMessage("\nEnter \"BEAM1\" to draw a beam.\n");
        }

        public void Terminate() {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Loads the ADACX Main Form
        /// </summary>
        [_AcRx.CommandMethod("BEAM1")]
        public static void CreateBeam() {
            _AcGe.Point3d p0, p1, pside;

            //
            // Get some user input:
            //
            _AcEd.Editor ed = _AcAp.Application.DocumentManager.MdiActiveDocument.Editor;

            _AcEd.PromptPointOptions ppo = new _AcEd.PromptPointOptions("\nSelect start point of beam: ");
            ppo.UseBasePoint = false;

            _AcEd.PromptPointResult ppres = ed.GetPoint(ppo);
            if(ppres.Status != _AcEd.PromptStatus.OK) {
                return;
            }
            p0 = ppres.Value;

            ppo.UseBasePoint = true;
            ppo.BasePoint = p0;
            ppo.Message = "\nSelect end point of beam: ";
            ppres = ed.GetPoint(ppo);
            if(ppres.Status != _AcEd.PromptStatus.OK) {
                return;
            }
            p1 = ppres.Value;

            ppo.BasePoint = p1;
            ppo.Message = "\nSelect side of beam drawing direction (Press ESC for on centre line): ";
            ppres = ed.GetPoint(ppo);
            if (ppres.Status == _AcEd.PromptStatus.Cancel) {
                pside = new Teigha.Geometry.Point3d(); // send empty point, could do better here with keywords for the propmt.
            } else {
                pside = ppres.Value;
            }
            
            //
            // Create the beam:
            //
            BeamData beamData = new BeamData(200, 120, 6000, p0, p1, pside);
            BeamDrawer beamDrawer = new BeamDrawer(beamData);
            beamDrawer.DrawBeam();
        }

    }
}
