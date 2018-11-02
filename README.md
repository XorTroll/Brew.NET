# Brew.NET - Some homebrew tools written in C# and .NET - featuring NSPack (more soon!)

This project contains:

- Brew.NET, a C# library using hacPack tool (credits to The-4n, great work!) to build NSP packages (yeah, the project and one of the projects inside it have the same name)
- NSPack, a GUI front-end for the libraries mentioned above, to easily build NSP packages

Credits to:

- The-4n for his awesome work with custom NCA and NSP building
- Liam for helping me with NACP support

## NSPack - simple NSP package and NCA content builder

### Using the builder

- There are some basic elements you need to provide at least to be able to make a NSP:
  - Title ID: 16 hex characters (example: 0100CAFE1234BEEF) - any title ID should work.

  - Name: the title's name, which will be the same for all the languages.

  - Author: the author/developer's name, which will also be the same for all the languages.
  
  - Version: the version string (1.0.0, 2.0beta1), limited to 16 characters.

  - Product code: a simple code string (SMO's product code: LA-H-AAACA), no matter what does it have.

  - ExeFS directory: any title needs to have a ExeFS, which contains the compiled source code and the metadata NPDM.

  - Icon: provide any icon, as it will be resized to 256x256, otherwise the defaut one will be used. It's recommended to use an image which is 256x256 or bigger.

- Appart from those, there are some other optional features to add or customize:

  - RomFS: the directory containing extra files for the title.
  
  - Logo: the custom PNG and GIF images which are shown when booting a title.

  - Important / IPNotices / Support HTML: this three are legal information HTML documents, which can be accessed from the home menu.

  - Offline HTML: this HTML documents are useless when making homebrew NSPs, but can be used (if you know how) to make your own video players...

  - Screenshots: Will the title allow taking screenshots?

  - Video: Will the title allow recording gameplay?

  - User account: Will the title ask for a user account when booting it?

- Using asset files

  - You can also save the assets as a asset file (*.nsxml format) if you are going to use that as a template for making NSPs.

- Planned stuff

  - Allow to load NACP files directly

  - Direct conversion from NRO to NSP, or any easy way to create NRO forwarders

  - Conversion from XCI to NSP

  - Add updates or add-on content support

- IMPORTANT

  - Installing and running NSPs can get you banned. Although this NSPs are not titlekey-encrypted (ovbiously), using them can be dangerous.

  - If the NSP doesn't run on your console, it can be for various reasons: used a title ID which is smallet than the NPDM's max title ID, not using a correct key generation...

  - Don't try building titles which ask for a user, because they could fail as generated NSPs don't ask for a user.

  - For the build process the program creates a temporary directory on the same folder as the EXE. Don't try messing up with that folder or deleting it while the NSP making process, because it could crash the program. Anyway, reopening the program should reset the directory.

  - As this program is made using WPF / PresentationFramework, it won't probably work on any other OSs but Windows (Wine supports Windows Forms but doesn't support WPF)