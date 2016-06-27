# DataChest
DataChest is *CLI(Command-Line Interface, same as CUI)* styled application. This allows you to encrypt and decrypt files on your computer.<br />
CLI styled application is not easy to use about normal users, so *GUI styled application is now on developing*.<br />

# Background
I started this project few months ago and this type of application is on all over the world.
Yes, I know this project is not unique and not rare, but there's reason. I wanted to make my own project and manage the project via version control system like GitHub. That's why I started and push this project on GitHub :D.

# Feature
- Encrypt and decrypt files with symmetric algorithms. (such as AES, DES, ... and etc)
- Users can set own password and IV(Initial Vector) for encryption and decryption.
- You can write your own DCF(DataChest File) format header to overriding `HeaderBase` class.
- Performance logging available with `--verbose` or `-v` option.
- You can write own comment on your files which be encrypted. (header version 2 or higher)
- Lightweight application.
- No additional libraries required.
- Xml comments for more easy to understand source code.

# Requirements
- .NET Framework 4.0 or higher.

# Improvements (TODO list)
- Minimize memory usage for optimizing.
- Multi-language support. (Currently only the Korean and English language are supported)
- Apply application icon.

# Options
- `-a <alg>, --algorithm=<alg>`
> Determine which algorithm used in cryptographic process. (Default is 'AES')

- `-A, --listalg`
> Show list of algorithms available.

- `-b <size>, --bufsize=<size>`
> Determine size of buffer used in cryptographic process. (Default is 4096, this value must higher or equal than 128)

- `-c, --cleanup`
> Delete input file after cryptographic process if succeed.

- `-d, --decrypt`
> Decrypt an encrypted file.

- `-D, --disableverify`
> Disable checksum verification.

- `-h <ver>, --headerversion=<ver>`
> Determine version of header to be used. (Currently only 1 and 2 is available)

- `-i <iv>, --iv=<iv>`
> Sets an IV(Initial Vector) used in cryptographic process.

- `-I, --infoheader`
> Display header information of input file.

- `-m <text>, --comment=<text>`
> Sets a comment for file. (This option only available on encryption)

- `-o <out>, --out=<out>`
>Sets an output file.

- `-v, --verbose`
> Display progress of cryptographic process verbosely in console.

- `-V, --version`
> Display version and license information in console.

- `-t, --test`
> Run the test about encrypted file. (This option cannot used with `-c, --cleanup` and `-w, --overwrite` options)

- `-w, --overwrite`
> Overwrite output file.

- `-p <pw>, --password=<pw>`
>Sets a password used in cryptographic process.

- `-?`
> Display usage of DataChest application.