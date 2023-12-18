namespace Pustok.PaginationHelper
{
	public class PaginatedList<T> : List<T>
	{
		public PaginatedList(List<T> datas, int count, int page, int pageSize)
		{
			this.AddRange(datas);
			ActivePage = page;
			TotalPageCount = (int)Math.Ceiling(count / (double)pageSize);
		}

		public int ActivePage { get; set; }
		public int TotalPageCount { get; set; }
		public bool HasNext
		{
			get => ActivePage < TotalPageCount;
		}
		public bool HasPrev
		{
			get => ActivePage > 1;
		}
		public static PaginatedList<T> Create(IQueryable<T> datas, int page, int pageSize)
		{
			return new PaginatedList<T>(datas.Skip((page - 1) * pageSize).Take(pageSize).ToList(), datas.ToList().Count, page, pageSize);
		}
	}
}