﻿namespace SyncPro.Runtime
{
    using System;
    using System.Diagnostics;

    using SyncPro.Adapters;
    using SyncPro.Data;

    public enum EntryUpdateState
    {
        Undefined,
        NotStarted,
        Succeeded,
        Failed
    }

    /// <summary>
    /// Contains information about the changes being made to an entry (file/folder) such as the
    /// type of change (add/delete/update/etc), the state of the change (NoStarted/Failed/Succeeded),
    /// modified metadata (timestamps/size/hashes), and any errors that resulted.
    /// </summary>
    /// <remarks>
    /// The lifetime of this object typically starts during the analyze stage when a change is first 
    /// detected in an entry. This object is then persisted until it is applied by the entry being
    /// synchronized, or when the analysis result is disposed (and the change is abandoned).
    /// </remarks>
    [DebuggerDisplay("{Entry.Name}")]
    public class EntryUpdateInfo : ISyncEntryMetadataChange
    {
        /// <summary>
        /// Then entry being changed.
        /// </summary>
        public SyncEntry Entry { get; }

        /// <summary>
        /// The path of the entry relative to the root that the entry is synced from.
        /// </summary>
        public string RelativePath { get; }

        /// <summary>
        /// The adapter that the change originated from.
        /// </summary>
        public AdapterBase OriginatingAdapter { get; }

        /// <summary>
        /// Flags indicating the type of change to be applied (added/updated/deleted/etc).
        /// </summary>
        public SyncEntryChangedFlags Flags { get; }

        /// <summary>
        /// The state of the change (whether it has been applied/succeeded).
        /// </summary>
        public EntryUpdateState State { get; set; }

        /// <summary>
        /// The error message that resulted from applying the change.
        /// </summary>
        public string ErrorMessage { get; set; }

        // The region below is the implementation of the ISyncEntryMetadataChange members
        // for tracking the metadata changes for a sync entry. If there are any missing 
        // members, simply copy this same block from the SyncHistoryEntryData class.
        #region Metadata Properties

        /// <summary>
        /// The previous size in bytes of the entry (if changed)
        /// </summary>
        public long SizeOld { get; set; }

        /// <summary>
        /// The size of the entry (in bytes) at the time when it was synced.
        /// </summary>
        public long SizeNew { get; set; }

        /// <summary>
        /// The previous SHA1 Hash of the file content (if changed)
        /// </summary>
        public byte[] Sha1HashOld { get; set; }

        /// <summary>
        /// The SHA1 Hash of the file content at the time when it was synced.
        /// </summary>
        public byte[] Sha1HashNew { get; set; }

        /// <summary>
        /// The previous MD5 Hash of the file content (if changed)
        /// </summary>
        public byte[] Md5HashOld { get; set; }

        /// <summary>
        /// The MD5 Hash of the file content at the time when it was synced.
        /// </summary>
        public byte[] Md5HashNew { get; set; }

        /// <summary>
        /// The previous CreationTime of the entry (if changed)
        /// </summary>
        public DateTime? CreationDateTimeUtcOld { get; set; }

        /// <summary>
        /// The CreationTime of the entry at the time it was synced.
        /// </summary>
        public DateTime? CreationDateTimeUtcNew { get; set; }

        /// <summary>
        /// The previous ModifiedTime of the entry (if changed)
        /// </summary>
        public DateTime? ModifiedDateTimeUtcOld { get; set; }

        /// <summary>
        /// The ModifiedTime of the entry at the time it was synced.
        /// </summary>
        public DateTime? ModifiedDateTimeUtcNew { get; set; }

        /// <summary>
        /// The previous full path of the item (from the root of the adapter) if changed.
        /// </summary>
        public string PathOld { get; set; }

        /// <summary>
        /// The full path of the item (from the root of the adapter) when it was synced.
        /// </summary>
        public string PathNew { get; set; }

        #endregion

        public bool HasSyncEntryFlag(SyncEntryChangedFlags flag)
        {
            return (this.Flags & flag) != 0;
        }

        public EntryUpdateInfo(SyncEntry entry, AdapterBase originatingAdapter, SyncEntryChangedFlags flags, string relativePath)
        {
            Pre.ThrowIfArgumentNull(entry, "entry");
            Pre.ThrowIfArgumentNull(originatingAdapter, "originatingAdapter");

            this.Entry = entry;
            this.OriginatingAdapter = originatingAdapter;
            this.Flags = flags;
            this.RelativePath = relativePath;

            if (flags != SyncEntryChangedFlags.None)
            {
                this.State = EntryUpdateState.NotStarted;
            }
        }

        public void SetOldMetadataFromSyncEntry()
        {
            this.SizeOld = this.Entry.Size;
            this.Sha1HashOld = this.Entry.Sha1Hash;
            this.Md5HashOld = this.Entry.Md5Hash;
            this.CreationDateTimeUtcOld = this.Entry.CreationDateTimeUtc;
            this.ModifiedDateTimeUtcOld = this.Entry.ModifiedDateTimeUtc;
            this.PathOld = this.Entry.UpdateInfo.RelativePath;
        }

        public void SetNewMetadataFromSyncEntry()
        {
            this.SizeNew = this.Entry.Size;
            this.Sha1HashNew = this.Entry.Sha1Hash;
            this.Md5HashNew = this.Entry.Md5Hash;
            this.CreationDateTimeUtcNew = this.Entry.CreationDateTimeUtc;
            this.ModifiedDateTimeUtcNew = this.Entry.ModifiedDateTimeUtc;
            this.PathNew = this.Entry.UpdateInfo.RelativePath;
        }
    }
}