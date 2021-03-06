﻿namespace SyncPro.Adapters
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using SyncPro.Utility;

    /// <summary>
    /// Represents an item exposed by an adapter, such as a file or directory.
    /// </summary>
    [DebuggerDisplay("{" + nameof(FullName) + "}")]
    public abstract class AdapterItem : IAdapterItem
    {
        /// <summary>
        /// The discrete name of the item (e.g. file name or directory name).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The unique ID of the item.
        /// </summary>
        public string UniqueId { get; }

        /// <summary>
        /// The type of the item (either a directory or a file).
        /// </summary>
        public SyncAdapterItemType ItemType { get; }

        public long Size { get; }

        public DateTime CreationTimeUtc { get; }

        public DateTime ModifiedTimeUtc { get; }

        public byte[] Sha1Hash { get; set; }

        public byte[] Md5Hash { get; set; }

        /// <summary>
        /// The adapter item that is the parent of this item.
        /// </summary>
        public IAdapterItem Parent { get; }

        /// <summary>
        /// The adapter where this item originated from.
        /// </summary>
        public AdapterBase Adapter { get; }

        public string ErrorMessage { get; protected set; }

        protected AdapterItem(
            string name, 
            IAdapterItem parent, 
            AdapterBase adapter, 
            SyncAdapterItemType itemType, 
            string uniqueId,
            long size,
            DateTime creationTimeUtc,
            DateTime modifiedTimeUtc)
        {
            this.Name = name;
            this.Parent = parent;
            this.Adapter = adapter;
            this.ItemType = itemType;
            this.UniqueId = uniqueId;
            this.Size = size;
            this.CreationTimeUtc = creationTimeUtc;
            this.ModifiedTimeUtc = modifiedTimeUtc;
        }

        private string fullName;

        /// <summary>
        /// The full path of the item.
        /// </summary>
        public string FullName => this.fullName ?? (this.fullName = GetPath());

        protected virtual string GetPath()
        {
            IAdapterItem folder = this;
            List<string> resultList = new List<string>();
            while (folder != null)
            {
                resultList.Add(folder.Name);
                folder = folder.Parent;
            }
            resultList.Reverse();
            return PathUtility.Join(this.Adapter.PathSeparator, resultList);
        }
    }
}