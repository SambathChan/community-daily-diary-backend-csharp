if (!db.getCollectionNames().includes("Posts")) {
    db.createCollection("Posts");
    print("Collection 'Posts' created!");
} else {
    print("Collection 'Posts' already exists.");
}
