# Wurkset
A simple, slow, file-based workset library with crude version control.

A workset is a unit of work.

Wurkset lets you define a unit of work with a POCO.  You can easily create and save worksets to a filesystem.  You can enumerate over the full collection and filter with traditional Linq.

Filesystem storage has the advantage of being large and cheap.  This comes at the tradeoff of speed.

* Be cautious about:
	* Concurrency.  You have to wait for the hard drive.  Consider injecting as a singleton.
	* Storing different classes in the same base directory.  Workset will enumerate faster if everything in the directory is the same class type.  It's simple to store each class in it's own directory.  Ex:
		* C:\Data\ClassA
		* C:\Data\ClassB
	* Cluster size. Filesystems store files in clusters.  A 1 b file could still consume 1 kb on disk due to how the filesystem is configured.
	
* FAQ:
 	* Why?
 		* Mostly I want to learn how to make a GitHub repo and this is a project I've thought about for a long time.  I started programming at a company that didn't have a database and I spent many years managing large sets of data exclusivly through the filesystem.  It's a good cheap solution for "cold data" that doesn't need fast access or as a stand in for a real repository during development.
	* How do I store extra files?
		* The Workset class provides the path to the workset.  Go nuts.  Just leave the nameof(T).json file alone.

# TODO
* [ ]Create IWorkset interface
* [ ]Add "Rename"
	* Ex: Rename class from A to B
* [ ]Add Optional Index
	* Should be able to tag a property in the class with an attribute that will mark it as an index.
	* Then app will index on that attribute for faster searching
		* This is hard, but worth it for long term.
