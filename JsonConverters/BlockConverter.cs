using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using SlackPrBot.Models.Slack.Api.Base.Blocks;

namespace SlackPrBot.JsonConverters
{
    internal class BlockConverter : JsonConverterBase<IBlock>
    {
        public override IBlock Create(Type objectType, JObject jObject)
        {
            var jProperty = jObject.Property("type");

            return jProperty.Value.ToString() switch
            {
                "section" => new SectionBlock(),
                "actions" => new ActionBlock(),
                "context" => new ContextBlock(),
                "divider" => new DividerBlock(),
                "input" => new InputBlock(),
                _ => throw new NotImplementedException("Type not implemented"),
            };
        }
    }
}
