using System;
using System.Collections.Generic;
using LaunchSitecore.Models;
using Sitecore.Analytics;
using System.Linq;
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
		public IEnumerable<GenericLink> RecommendedPages
		{
			get
			{
				var recommendedPages = new List<GenericLink>();

				if (Tracker.Current.Interaction.Pages.Length <= 1)
					return recommendedPages;

				var currentSequence = Tracker.Current.Interaction.Pages.OrderBy(p => p.VisitPageIndex).Select(p => p.Item.Id);

				var treeManager = new TreeManager(ApplicationContainer.GetTreeProvider(), ApplicationContainer.GetTreeCache());
				var tree = treeManager.GetTree(Guid.Parse("{68E713D8-A382-4378-8FB0-9D7F7AD14B25}"), DateTime.UtcNow.AddDays(-7), DateTime.UtcNow.AddDays(1));
				if (tree?.Root == null)
				{
					return recommendedPages;
				}
				
				var allRecommendations = GetAllPossibleRecommendations(tree.Root, currentSequence.ToArray()).ToList();

				//NOTE: this is where we could filter the recommendations further
				//currently just ordering by subtree count descending
				foreach (var page in allRecommendations.OrderByDescending(p => p.SubtreeCount))
				{
					var item = Sitecore.Context.Database.GetItem(ID.Parse(page.RecordId));
					if (item == null)
					{
						continue;
					}
					
					var name = item.DisplayName;
					var url = LinkManager.GetItemUrl(item);

					recommendedPages.Add(new GenericLink(name, url, true));
				}
				
				return recommendedPages;
			}
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

			var firstPageId = pageSequence.FirstOrDefault();

			var startNodes = startNode.Nodes.Cast<PageNode>().Where(n => n.RecordId == firstPageId);
			
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
					//match! continue processing children
					ProcessChildren(index + 1, pageSequence, child, recommendations);
				}
			}
		}
	}
}
