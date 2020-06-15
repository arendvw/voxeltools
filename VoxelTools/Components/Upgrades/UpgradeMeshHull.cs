using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using StudioAvw.Voxels.Components.VoxelGrid;
using StudioAvw.Voxels.Helper;

namespace StudioAvw.Voxels.Components.Upgrades
{
    /// <summary>
    /// A class implementing IGH_UpgradeObject to enable the "Upgrade component" menu to recognize out of date "Restore State" components.
    /// </summary>
    /// <seealso cref="Grasshopper.Kernel.IGH_UpgradeObject" />
    public class UpgradeMeshHull : IGH_UpgradeObject
    {
        /// <summary>
        /// Upgrade an existing object.
        /// </summary>
        /// <param name="target">Object to upgrade.</param>
        /// <param name="document">Document that contains the object.</param>
        /// <see>https://discourse.mcneel.com/t/how-to-handle-input-output-definition-changes-when-a-component-gets-updated-in-a-new-version/64635/5</see>
        /// <returns>
        /// The newly created object on success, null on failure.
        /// </returns>
        public IGH_DocumentObject Upgrade(IGH_DocumentObject target, GH_Document document)
        {
            // ReSharper disable once UseNegatedPatternMatching
            var component = target as IGH_Component;
            var comp = GH_UpgradeUtil.SwapComponents(component, UpgradeTo, true);
            if (comp == null)
            {
                return null;
            }
            var param = new Param_Boolean();
            param.PersistentData.Append(new GH_Boolean(false));
            param.Name = "Fake Shadow";
            param.NickName = "FS";
            param.Description = "Add fake shadow to the mesh, it can help to visualize the grid.";
            param.Access = GH_ParamAccess.item;
            comp.Params.Input.Add(param);
            return comp;
        }

        /// <summary>
        /// Gets the ComponentGuid of the old object (the object to be updated).
        /// </summary>
        public Guid UpgradeFrom => new Guid("{E99F9A6C-2568-40B4-9AE8-73423442BA96}");

        /// <summary>
        /// Gets the ComponentGuid of the new object (the object that will be inserted).
        /// </summary>
        public Guid UpgradeTo => new Guid("{2C2E9FB7-2018-4F5E-BDA9-C56A171709F0}");

        /// <summary>
        /// Return a DateTime object that indicates when this upgrade mechanism was written,
        /// so that it becomes possible to distinguish competing upgrade mechanisms.
        /// </summary>
        public DateTime Version => new DateTime(2020, 5, 25);
    }
}
