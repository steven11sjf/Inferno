# Inferno

Google Drive link: https://drive.google.com/drive/folders/162jZHDAdWzgksmENo4kC5yEXmS8ZVHbF

## Installation

Download and install [Git for Windows](https://gitforwindows.org/). Put the Git Bash program on your start menu, you'll use it a lot. 

Also download and install Unity hub. Get the personal version, it should be all you need. 

Open git bash. Make a folder somewhere (I recommend on the root of the C drive). You access the drive in git bash by typing `cd /c/`. For the purposes of this I will use a folder called `/c/Inferno`. Navigate into this directory in git bash. 

Make sure you're logged into git. Use `git config --global user.name "Steven Franklin"` (use your name though). also put in your email using `git config --global user.email <email>`. 

Now, to get a local repository type `git clone hyperbola0@github.com/hyperbola0/Inferno.git`. Enter your password when asked. It will clone a repository to your computer. 

You should be able to open the Unity hub and open the project, and it will download the correct version of Unity. Wait for that and double-click that to open the project. It takes a minute the first time while it generates some shaders and other files specific to your computer not in the repo. 

If you have issues DM me :^)

## Branches

Branches can be switched between using `git checkout <branch>`. It's a "version" of the code. `master` is the master-copy and should only be for release/stable versions. `dev` is the equivalent of a nightly build, containing completed features but may contain some bugs or unintended behaviors still.

## How to add changes

First, create an issue in github. Navigate to [the repository](www.github.com/hyperbola0/Inferno) and open the Issues tab. Create a new issue and add a title and description of what you plan to do. Don't forget to choose a label on the right side of the screen. Note the issue number. 

Next, make sure your local repository is up to date by navigation to `/c/Inferno` in git bash and running `git pull`. Now create a branch using `git checkout -b <new-branch>`. The branch naming convention we use is <label>/<issue-number>-<description>. For example, the branch I'm using for this documentation change is `documentation/10-readme-changes`. 

Next, implement your changes. This should be straightforward; open Unity and change whatever you were going to. Don't forget to save both the .cs files and the Unity scene (press ctrl-S in the editor). 

Now, commit your changes to the branch. Go to git bash and type `git commit` and it will open a vim file for you to edit. Ignore the commented lines with a # in front of them. Press `i` to enter edit mode. 

The commit format we're using is:

```
<label> (#<issue-number>): <description>

- individual changes
- try to be somewhat specific, especially if you changed something from your original intentions in the issue
```

For example:

```
documentation (#10): README fixes

- Added section explaining how to get a local git repo
- Added section explaining how branches work
- Added explanation for how to use issues and branches
```

When you're done editing the commit message, press escape to exit the editor and then type ":w" and enter to save, then ":q" and enter to quit the editor. You'll see git committing the message. 

Next, we need to push the change to the remote repository. Right now your changes only exist on your machine. Run `git push --set-origin origin <current-branch>`. This pushes it to github's copy, and you should see the commit referenced if you open your issues page. 

Finally, merge into dev. Checkout dev (`git checkout dev`) and run `git merge --no-ff <branch>`. If there are no errors you should see your changes merged into dev. If all has gone well, you can close the issue and be done. 
