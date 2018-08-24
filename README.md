# FakeDataUploader

# Notes

Replaced Seq with LazyList as Seq's performances were insanely poor, List was extremely fast but eager and wasting memory/time creating useless objects.