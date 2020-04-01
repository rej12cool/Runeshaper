# Runeshaper

## Description
Runeshaper is a singleplayer 2D puzzle-platformer video game to be played on a computer. It is a side scroller with
environmental elements that has the player exploring a set of ruins and using runes to help navigate through them. Runes
activate special environmental effects based on primitive energies/elements like air, water, earth, fire, and more. By
placing a rune on a surface, the elemental effect can either help or hinder you in during your exploration of the ruins.

## Guidelines
### Workflow
Each week, the team will have a meeting on Sundays at 3 pm EST. During this meeting, there will be a meeting leader, and
each week, the person leading the meeting changes. The leader will go around and ask the other team members what they
were tasked with completing, what they were able to complete, what they struggled with, and what they might need help in
the future. This part of the meeting should last 10-20 minutes.

Tasks that were not started will be put into the backlog and saved for later. The Trello cards associated with those
tasks will be put back into the backlog list. Tasks that are larger in scale or are more critical for others to work in
parallel can be continued into the next sprint.

After this point, the team will delegate together which tasks each person will attempt to complete. in general, most of
the tasks should be completed by the next meeting, so try to estimate exactly how much you can reasonably get done
within the week. Each item in the backlog should not be assigned to any team member, but when tasks for the weekly
meeting are divided up, the Trello cards should be moved from the backlog into the To Do list and assigned to that
person.

While working, if you feel like you need additional help or are running into difficulties, let the rest of the team
know. This way, we can all better understand what the problem is and make any necessary changes.

Each week, the GitHub and Trello pages should be viewed in case there are updates that require your attention. No team
member should have to wait multiple days before receiving comments on a pull request, etc. This is especially true
towards the end of the week when commits will be made before/during the weekly meetings.

### Unity
- The Unity version that will be used for the entirety of the project will be version `2019.3.6f1`.
- For the purposes of creating a build for testing, you only need to create the executable game for your specific
  platform (PC or Mac). Before submitting the build, both builds for PC and Mac should be created and then uploaded.

### Coding Standards
For the most part, all of the code will be written in C# for the Unity based scripting. Most of the coding standards for
this project will adhere to the C# standards or just general good coding practices. Everything else is just so that we
can be consistent. These rules can be updated at any time.
1. All variables start with a lowercase letter in camelcase format: `gameObject`
2. All methods should start with a capital letter in camelcase format `void Update()` or calling `PerformAction(action)`
3. All curly braces will start on a new line and will not be on the same line. Bad: `for (int i = 0; i < MAX_ITER; i++)
   {`
4. A thorough comment will be given at the beginning of all non-trivial methods to include a description of the function
   as well as the meanings of all of the parameters and return type, if any.
5. All variables that are primitive types must begin with the first letter of the primitive data type. An integer might
   be `iNumEnemies` or a string would be `sCharacterName`. This rule does not apply to variables that are not primitive
   types.
6. All variables should be given proper naming schemes to describe their function. Intermediate values like `i` and `j`
   for `for` loops are acceptable, but variables names like `temp` should never be used. If the iterated variable could
   have a better name, it should be used instead.
7. All member variables (global scope) should start with `m_` if they are not declared `const` and should be in all caps
   with underscores in between words if they are constant. Example: `m_iCollectiblesOnScreen`, `m_bIsCharacterAirborne`,
   `MAX_ENEMIES_ON_SCREEN`, `GRID_SIZE`
8. All member variables should have the lowest amount of access necessary. Essentially, not all variables should be
   `public` if they don't need to be.
9.  Lines should not extend beyond 120 characters, which includes both functional code and comments.

### GitHub and Trello
- Every time a feature is being updated, a new GitHub branch should be opened. Even for small changes. For every feature
  branch, a new Trello card should be associated with it. After creating a branch in GitHub, in Trello, attach the
  feature branch to the card related to the task. This can be done by clicking on the card so it is a large pop-up
  within Trello, going to the GitHub Power-Up (listed on the right side) and then attaching a branch.
- When making a commit, make sure to write a good description to briefly state what changes were made in the commit. Do
  not commit to `master` directly unless under specific circumstances.
- When the branch is ready to be merged back into `master`, finish making any commits and then open a pull request (PR).
  Move the Trello card associated with the branch into the Completed/In Review list and attach the PR to the card,
  similar to attaching the branch above. PRs should also have all other team members listed as a reviewer to do a code
  review (CR). At least one CR is required for any merges to be made into `master`, but more is better.
- After a branch has been merged in, you should re-fetch the repository to get any changes and try building the game for
  your PC/Mac. Once you can confirm that the build works as intended, you should move the Trello card from the In
  Review/Completed list into the Done list.
- No branches should be merged in between the weekly stand-up meeting and the time of posting the new build to Piazza.
  If we are able to submit a stable build before the Tuesday deadline, we can continue to merge branches in.
- The branch should be deleted after getting merged in.

### Code Reviews
- In order for a branch to be merged into `master`, a PR must be opened and at least one CR must be performed. If the CR
  looks fine, the associated branch can be merged into master.
- When doing a CR, ensure that all of the coding guidelines are strictly adhered. If any violations to the standards are
  present, an inline comment with specific instructions should be given. If multiple violations occur in the same file,
  make a single comment and list the other line numbers where changes need to be made. If there are many violations,
  make a comment on the file with exactly which guidelines should be adhered to.
- While reading someone else's code, it should be clear what each method/piece of code is doing. If it is not inherently
  clear, and is missing a clarifying comment, make a not in the CR. Whatever code is written should be reasonably easy
  to understand by someone who didn't work on it.
- When the CR is done, write an overall comment before submitting it and check off the status (needs response, accept,
  or needs work)
- Even though only 1 CR is required per PR, more people are encouraged to review it.
