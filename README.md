# Memory-File-System
## Description
In this project I implemented a TinyMemFS (memory file system) object, which simulates the file system of the operating system. And in addition, I created an application for the user so that he can easily manage the functionality of the object. The main functionality that I implement for the system is: adding files, saving files on disk, encryption\decription and displaying the files that are in the system at a given time.

## Functionality And Logics
* I used two dictinary- the first dictinary called headers, contain the name of the file as a key and as a value, path file and data (file name, length, date of creation).
the second dictinary, called content, contain the name of the file as a key and as a value, list of bytes that represent the content of the file.
The separation between the dictionaries is to facilitate the encryption and to save runtime when accessing the text of the file.
* Add function- add file to headers dictinary file name is the key, data (name, length, date) is the value.
* Remove function- remove fileName from the system, this operation releases all allocated memory for this file/
* encrypt function- encrypt the text in the file with a key that the user gives, Possibility of multiple encryptions.
* decrypt function- decrypt the text in the file with same encryption key.
