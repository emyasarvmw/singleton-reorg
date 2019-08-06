# Contributring to Singleton-demo

## Welcome

Welcome ......


## Getting Started

### Fork Repository and clone

People can only commit changes on fork repository then open a pull request to Singleton repository, so please fork this repository first.

Now, you can clone your forked repository to local with this command:
git clone git@github.com:<git_account>/singleton.git

### Branch

We have sub projects in each branch:

|      branch    | purpose |
|----------------|---------|
|   master       | Singleton service code  |
|   java-client  | Singleton java client code  |
|   js-client    | Singleton js client code  |
|   devops       | CI and automation testing code, it is not place to contribute code unless you want to add CI or test scripts.  |

Changes should be made on your own forked branch. PR should be rebased on top of one of above branches without multiple branches mixed into the PR. If your PR do not merge cleanly, use commands listed below to get it up to date.

```
#upstream is the origin upstream(refer to https://help.github.com/en/articles/configuring-a-remote-for-a-fork for how to configure upstrea

cd $working_dir/singleton
git fetch upstream
git checkout master
git rebase upstream/master
```

Branch from the updated `master` branch:

```
git checkout -b my_feature master
```

Accordingly, for each client please checkout to its branch.


## Contribute flow

### Fork Repository

Go to https://github.com/vmware/singleton and click 'Fork' on top of page.

### Set Signature

Set Signature in Github, refer https://help.github.com/en/articles/about-commit-signature-verification#gpg-commit-signature-verification

### Setup CI Pipeline
Singleton use Travis-ci as CI framework and help contributor to test their code change as early as possible. Some configuration need to be done to setup CI pipeline in your fork repository.

##### SonarCloud Configuration

1. Generate Sonar Token

    a. After you fork the repositpry, login to 'sonarcloud.io' with your github account;
    
    b. My Account -> Security, generate a token and copy it, it will be used later.
    
2. Get Organization Key

    a. Click your icon and go to 'My Organizations', click Create -> Import from GitHub -> Choose an organization on Github -> select your personal account -> click 'install', now you will have your personal organization on SonarCloud.

    b. Click your icon again and select your personal organization, mark down the key of your personal organization in right-up corner(not Organization name), it will be used in Travis CI configuration later.

##### Travis CI Configuration

1. After you configured SonarCloud, login to Travis-ci 'https://travis-ci.com' with your github account. If this is your first time to integrate travis to Github, you need to click 'Active' to apply the Github apps integration. (If you fork singleton to some organization, please select the organization first then click 'Active'). After all done, you could see singleton is list in repositories tab. 

2. Find your fork repository in your account settings, click 'settings', add following environment variables
    
    SONAR_ORG=\<Organization key in SonarCloud configuration step2\>
    
    SONAR_TOKEN=\<token generated in SonarCloud\>
    
    (Note: if you disable 'DISPLAY VALUE IN BUILD LOG', the CI will be failed for PR from another forked repo to your repo. But it will not impact your PR from your forked repo to vmware/singleton repo.)
    
##### CI Pipeline Result

After you finished above configurations, you can commit and push code to your fork repository. It will trigger CI on Travis, you can login Travis to see result. You can login SonarCloud.io to see code scan details.

The sonar project and quality gate will be created automatically, project name should like: <SONAR_ORG>:\<sub-project name\>:\<branch>. Quality gate name should like: singleton:\<sub-project name\>-gate

#### Vulnerable Scan

We use Dependabot Preview(A Github APP owned by Github which provide dependencies scan) to do vulnerable scan

#### Configuration

1. Login to Github, click 'Marketplace' in navigation on top.

2. Search for 'Dependabot Preview' and click the search result.

3. Click 'Set up a plan', then the screen scroll down to the end of the page, find 'Install it for free' and click it, then click 'Complete order and begin installation', then click 'Install' to install the Github APP.

4. Click 'Sign in with Github', then click 'Authorize Dependabot Preview by GitHub' to sign in Dependabot Preview. After that, you will able to see the Dependabot Preview panel.

5. Click 'Select repos to add' and fill in the repo, dependencies description file(e.g. pom.xml for Maven, build.gradle for Gradle, package.json for NPM) info in the popup modal, then click 'Add language' to finish the set up.

6. After the set up, the Dependabot Preview will scan the open dependencies in your project automatically, if any vulnerability found, pull requests with detailed info and auto fix will be created for the repo, please keep eye on it.

### Pull Request

You can open a PR after you get pass result of CI pipeline. CI pipeline will run again for this PR, then you can wait for review and merge.

#### Syncing a fork

Please sync your fork repo with upstream repo before you create PR, refer to https://help.github.com/en/articles/syncing-a-fork

### Develop, Build and Test

#### Singleton Service

For Singleton service feature development, you can use IntelliJ IDEA or Eclipse to do development work and import the codes as gradle project. The coding style follow [JAVA style](https://petroware.no/javastyle.html).

To build the code, you can use gradle's build task. After the build task is finished, you can find the build under the project '/build/libs/' path.

Unit test cases should be added to cover the new code.

#### Singleton Client

For each specific client, it depends on the programing lanunages to use the according development tool, please follow the individual guidline.