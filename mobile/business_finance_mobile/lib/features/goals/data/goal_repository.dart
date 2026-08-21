import '../../../core/models/data_choice.dart';
import '../../../core/models/json_readers.dart';
import '../../../core/network/api_client.dart';
import 'goal_models.dart';

abstract interface class GoalRepositoryContract {
  Future<GoalsSnapshot> load(String asOfDate);
  Future<void> create(Map<String, Object?> input);
  Future<void> delete(String goalId);
  Future<void> contribute(String goalId, Map<String, Object?> input);
}

class GoalRepository implements GoalRepositoryContract {
  const GoalRepository(this._client);
  final ApiClient _client;

  @override
  Future<GoalsSnapshot> load(String asOfDate) async {
    final responses = await Future.wait([
      _client.get('/api/v1/goals?asOfDate=$asOfDate'),
      _client.get('/api/v1/accounts?pageNumber=1&pageSize=100&isActive=true'),
    ]);
    return GoalsSnapshot(
      goals: _items(
        responses[0].requireObject(),
      ).map(GoalItem.fromJson).toList(growable: false),
      accounts: _items(
        responses[1].requireObject(),
      ).map(DataChoice.fromJson).toList(growable: false),
    );
  }

  @override
  Future<void> create(Map<String, Object?> input) async =>
      _client.post('/api/v1/goals', body: input);

  @override
  Future<void> delete(String goalId) async =>
      _client.delete('/api/v1/goals/$goalId');

  @override
  Future<void> contribute(String goalId, Map<String, Object?> input) async =>
      _client.post('/api/v1/goals/$goalId/contributions', body: input);

  List<Map<String, dynamic>> _items(Map<String, dynamic> json) =>
      JsonReaders.list(
        json,
        'items',
      ).map((item) => JsonReaders.object(item, 'item')).toList(growable: false);
}
