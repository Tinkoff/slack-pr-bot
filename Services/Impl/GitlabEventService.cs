using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using SlackPrBot.Models.Gitlab.Events;

namespace SlackPrBot.Services.Impl
{
    internal class GitlabEventService : IGitlabEventService
    {
        private readonly IStashEventService _stashEventService;
        private readonly GitlabMergeRequestEventsConverter _gitlabMergeRequestEventsConverter;
        private readonly GitlabNoteConverter _gitlabNoteConverter;

        public GitlabEventService(IStashEventService stashEventService, GitlabMergeRequestEventsConverter gitlabMergeRequestEventsConverter, GitlabNoteConverter gitlabNoteConverter)
        {
            _stashEventService = stashEventService;
            _gitlabMergeRequestEventsConverter = gitlabMergeRequestEventsConverter;
            _gitlabNoteConverter = gitlabNoteConverter;
        }

        public async Task GitlabEventAsync(JObject eventData)
        {
            var objectKind = eventData.Value<string>("object_kind");
            if (objectKind == "merge_request")
            {
                var model = eventData.ToObject<MergeRequestEvent>();
                var result = _gitlabMergeRequestEventsConverter.Convert(model);
                if (result != null)
                {
                    await _stashEventService.StashEventAsync(result);
                }
            }

            if (objectKind == "note" && eventData["merge_request"] != null)
            {
                var model = eventData.ToObject<NoteEvent>();
                var result = _gitlabNoteConverter.Convert(model);

                await _stashEventService.StashEventAsync(result);
            }
        }
    }
}