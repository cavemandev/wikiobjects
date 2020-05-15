# wikiobjects

The design is divided into two sections, a Controller Library and Models + Model Interface

The Models show what fields exists, although I didn't really add anything other than name to the Models. The models are written to MongoDB.

The controller gets access to a "lite" version of each model through the respective ModelInterface. The lite version doesn't have MongoDB access. I don't like allowing DB access to arbritary parts of program, so each Model is tranformed to it's lite version before being passed to the controller.

All Permissions gating is done at the controller level. This allows the models to be used by update programs, or super user programs that need full admin control over everything. If a rest API where to be created, it would go through the controllers, and would be gated that way.

Assumptions:
When we delete a page, we don't delete all the subpages and attachments
Teams can be added to teams
