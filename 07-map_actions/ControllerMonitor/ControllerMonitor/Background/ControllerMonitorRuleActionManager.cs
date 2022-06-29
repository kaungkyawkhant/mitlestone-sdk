using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using VideoOS.Platform;
using VideoOS.Platform.Data;
using VideoOS.Platform.RuleAction;

namespace ControllerMonitor.Background
{
    class ControllerMonitorRuleActionManager : ActionManager
    {

        private static Guid OpenGateActionItemId = new Guid("CDA1082F-852F-4A7C-B966-C76150C0EEE3");
        private static Guid CloseGateActionItemId = new Guid("D775C8D5-009A-4DA8-8583-D3C3706870CA");
        private readonly ControllerMonitorDefinition _definition;

        public ControllerMonitorRuleActionManager(ControllerMonitorDefinition definition)
        {
            _definition = definition;
        }

        public override void ExecuteAction(Guid actionId, Collection<FQID> actionItems, BaseEvent sourceEvent)
        {
            if (actionId == OpenGateActionItemId)
            {
                foreach (FQID fqid in actionItems)
                {
                    // Execute action
                    Item item = Configuration.Instance.GetItemConfiguration(ControllerMonitorDefinition.ControllerMonitorPluginId, ControllerMonitorDefinition.ControllerMonitorKind, fqid.ObjectId);
                    // either signal background plugin to send command on already established channel or simply do it here yourself
                    _definition.ControllerMonitorBackgroundPlugin.SendCommand(item, "OPEN");
                }
            }

            if (actionId == CloseGateActionItemId)
            {
                foreach (FQID fqid in actionItems)
                {
                    // Execute action
                    Item item = Configuration.Instance.GetItemConfiguration(ControllerMonitorDefinition.ControllerMonitorPluginId, ControllerMonitorDefinition.ControllerMonitorKind, fqid.ObjectId);
                    // either signal background plugin to send command on already established channel or simply do it here yourself
                    _definition.ControllerMonitorBackgroundPlugin.SendCommand(item, "CLOSE");
                }
            }
        }

        public override Collection<ActionDefinition> GetActionDefinitions()
        {
            // Expose supported actions here
            return new Collection<ActionDefinition>()
            {
                new ActionDefinition()
                {
                    Id = OpenGateActionItemId,
                    Name = "Open gate",
                    SelectionText = "Open gate using <controller>",
                    DescriptionText = "Open gate using {0}",
                    ActionItemKind = new ActionElement()
                        {
                            DefaultText = "controller",
                            ItemKinds = new Collection<Guid>() { ControllerMonitorDefinition.ControllerMonitorKind }
                        }
                },
                new ActionDefinition()
                {
                    Id = CloseGateActionItemId,
                    Name = "Close gate",
                    SelectionText = "Close gate using <controller>",
                    DescriptionText = "Close gate using {0}",
                    ActionItemKind = new ActionElement()
                        {
                            DefaultText = "controller",
                            ItemKinds = new Collection<Guid>() { ControllerMonitorDefinition.ControllerMonitorKind }
                        }
                }
            };
        }
    }
}
