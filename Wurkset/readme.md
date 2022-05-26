﻿# Wurkset #
A simple, slow, file-based workset library with crude version control.

A workset is a unit of work.

Wurkset lets you define a unit of work with a POCO.  You can easily create and save worksets to a filesystem.  You can enumerate over the full collection and filter with traditional Linq.

Filesystem storage has the advantage of being large and cheap.  This comes at the tradeoff of speed.


* Avoid:
	* Concurrency with Wurkset.  You have to wait for the hard drive.  Instead inject as a singleton.
	* Storing different classes in the same base directory.  Enumerating a repository with Wurkset will deserialize each class.  Instead store different classes in different base directores.
	
	
* Be careful about:
	* Cluster size. Filesystems store files in clusters.  A 1 b file could still consume 1 kb on disk due to how the filesystem is configured. Format the disk to match your use case... or don't.  Disk space is cheap.  That's the point.
	
* FAQ:
	* How do I store extra files?
		* The Workset class provides the path to the workset.  Go nuts.  Just leave the data.json file alone.

# TODO
Create IWorkset
Rename WorksetRepository to WorksetManager
Write demo app