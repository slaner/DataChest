This README is not completed written and need your help for improve README and DataChest project.

---
DataChest
=
DataChest application makes you easily encrypt sensitive files. Oh! it's also support decrypt the encrypted one.

Background
=
I started this project few months ago and this type of application is on all over the world.
Yes, I know this project is not unique and not rare, but there's reason. I wanted to make my own project and manage the project via version control system like GitHub. That's why I started and push this project on GitHub :D.

README
=
First of all, I apologize for my bad English skills.
You may not understand what I trying to tell you, but I'll try hard to improve my English skills.

License
=
Copyright (c) 2016 HYE WON, HWANG

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.

Options
=
`-a <alg>, --algorithm=<alg>`
>Specify algorithm that used in encryption and decryption process. Default is 'Aes'

`-A, --listalg`
>Show list of algorithms can be used.

`-c, --cleanup`
>Delete input file when task succeed.

`-d, --decrypt`
>Decrypt a encrypted file.

`-D, --disableverify`
>Disable checksum(crc) verification.

`-i, --infoheader`
>Display CHEST_HEADER information of encrypted file.

`-I <iv>, --iv=<iv>`
>Sets Initial Vector(IV) that used in encryption and decryption process.

`-o <out>, --out=<out>`
>Sets output file.

`-v, --version`
>Display version and license information.

`-V <ver>, --apiversion=<ver>`
>Sets ChestAPI version.

`-t, --test`
>Run the test. This option will not generate output file.

`-W, --overwrite`
>If output file already exists, will overwrite exist file.

`-p <pw>, --password=<pw>`
>Sets Password.

`-?`
>Display usage.