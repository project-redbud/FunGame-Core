using FunGame.Core.Model.Framework;

namespace FunGame.Core.Controller
{
    public partial class AIController
    {
        internal void BindMap(GameMap? map)
        {
            if (ReferenceEquals(_map, map)) return;
            _map = map;
            ClearTransientState();
        }

        internal void ClearTransientState()
        {
            _movementHistory.Clear();
            LastPolicyResult = null;
            LastMetrics = null;
            LastPlanningDiagnostics = [];
        }
    }
}
