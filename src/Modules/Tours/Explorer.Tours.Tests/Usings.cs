global using Xunit;
//test PreferenceTagsTests.GetTagsForPerson_Returns_existing_tags was failing due to deadlock
//this disables running test in parallel therefore fixing the issue
[assembly: CollectionBehavior(DisableTestParallelization = true)]