using System;
using System.Collections.Generic;
using LaunchSitecore.Models;
using Sitecore.Analytics;
using System.Linq;
using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.PathAnalyzer;
using Sitecore.PathAnalyzer.Data;
using Sitecore.PathAnalyzer.Data.Models;
using Sitecore.SequenceAnalyzer;

namespace MspUg.Web.Models
{
	public class Recommendations
	{
		public Dictionary<Guid, GenericLink> GetRecommendedPages()
		{
			var recommendedPages = new Dictionary<Guid, GenericLink>();

			if (Tracker.Current.Interaction.Pages.Length <= 1)
				return recommendedPages;

			var currentPageSequence = Tracker.Current.Interaction.Pages.OrderBy(p => p.VisitPageIndex).Select(p => p.Item.Id).ToArray();
			//this where statement removes consecutive duplicates in the sequence.
			//for instance, given this array of numbers: [1, 2, 2, 3, 1, 1, 5, 6, 6, 8]
			//we want to return: [1, 2, 3, 1, 5, 6, 8]
			//which is what we want to do for the page ID sequence. remove consecutive duplicates to filter out noise like page refreshes.
			currentPageSequence = currentPageSequence.Where((id, i) => i == 0 || (currentPageSequence.Length > i && id != currentPageSequence[i - 1])).ToArray();

			var treeManager = new TreeManager(ApplicationContainer.GetTreeProvider(), ApplicationContainer.GetTreeCache());
			//Use the PathAnalyzer API to retrieve the 'All Site Experience' tree for the last week.
			var tree = treeManager.GetTree(Guid.Parse("{68E713D8-A382-4378-8FB0-9D7F7AD14B25}"), DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(1));
			if (tree?.Root == null)
			{
				return recommendedPages;
			}
			
			//retrieve all possible recommendations
			var allRecommendations = GetAllPossibleRecommendations(tree.Root, currentPageSequence).ToList();

			var excludedItems = new[]
			{
				Guid.Parse("{110D559F-DEA5-42EA-9C1C-8A5DF7E70EF9}")
			};
			
			//NOTE: this is where we could filter the recommendations further
			//currently just ordering by combined subtree value and subtree count in descending order (most valuable first)
			//also filtering out the home item

			foreach (var pageNode in allRecommendations.OrderByDescending(p => p.SubtreeValue + p.SubtreeCount).Where(p => !excludedItems.Contains(p.RecordId)))
			{
				var id = pageNode.RecordId;
				if (recommendedPages.ContainsKey(id))
				{
					continue;
				}

				var item = Context.Database.GetItem(ID.Parse(id));
				if (item == null)
				{
					continue;
				}
					
				var name = item.DisplayName;
				var url = LinkManager.GetItemUrl(item);

				recommendedPages.Add(id, new GenericLink(name, url, false));
			}
				
			return recommendedPages;
		}

		public IEnumerable<PageNode> GetAllPossibleRecommendations(INode startNode, Guid[] pageSequence)
		{
			Assert.ArgumentNotNull(startNode, "startNode");
			Assert.ArgumentNotNull(pageSequence, "pageSequence");

			var recommendations = new List<PageNode>();

			if (!pageSequence.Any())
			{
				return recommendations;
			}

			/*
			The basic algorithm is this:
			- Given the pages from the current interaction as a sequence, we are attempting to find all matching sequences in the provided map.
			- When we find a match, we retrieve the 'next' pages in the sequence from the map. 
			- Those 'next' pages are considered possible recommendations, which we return from this method for further filtering.
			*/
			
			//extract the first page id from the pages in the current interaction
			var firstPageId = pageSequence.FirstOrDefault();

			//Find all nodes in the map that match the first page id
			var startNodes = startNode.Nodes.Cast<PageNode>().Where(n => n.RecordId == firstPageId);
			
			//iterate all the 'first page id' nodes, recursively processing child nodes to try to match the given page sequence
			foreach (var node in startNodes)
			{
				var possibleRecommendations = new List<PageNode>();
				ProcessChildren(1, pageSequence, node, possibleRecommendations);
				recommendations.AddRange(possibleRecommendations);
			}

			return recommendations;
		}

		private void ProcessChildren(int index, Guid[] pageSequence, PageNode current, List<PageNode> recommendations)
		{
			if (index >= pageSequence.Length)
			{
				//We've reached the end of the page sequence, so we know the whole page sequence has been matched.
				//This means the next possible nodes (children) can be considered recommendations.
				var children = current.Children.ToList();
				if (children.Count > 0)
				{
					recommendations.AddRange(children.Cast<PageNode>());
				}
				return;
			}

			foreach (var child in current.Children.Cast<PageNode>())
			{
				if (child.RecordId == pageSequence[index])
				{
					//we found a match! so continue processing children to determine if more matches occur.
					ProcessChildren(index + 1, pageSequence, child, recommendations);
				}
			}
		}
	}
}
