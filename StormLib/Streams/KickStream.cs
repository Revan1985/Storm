using System;
using System.IO;

namespace StormLib.Streams
{
	public class KickStream : StreamBase
	{
		private Uri? _icon = null;
		public override Uri Icon
		{
			get
			{
				if (_icon is null)
				{
					string path = Path.Combine(IconDirectory, "Kick.ico");

					_icon = new Uri(path);
				}

				return _icon;
			}
		}

		public override bool HasStreamlinkSupport { get => false; }

		public override string ServiceName { get => "Kick"; }

		public KickStream(Uri uri)
			: base(uri)
		{ }
	}
}